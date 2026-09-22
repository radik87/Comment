import { Component, ElementRef, inject, input, output, signal, viewChild } from '@angular/core';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { CommentsService } from '../services/comments.service';
import { ALLOWED_TAGS, SafeCommentPipe } from '../pipes/safe-comment.pipe';
import { Captcha } from '../captcha/captcha';

type FieldName = 'userName' | 'email' | 'homePage' | 'text' | 'captcha';

/** Только разрешённые теги, все закрыты в правильном порядке (валидный XHTML) */
function validTags(control: AbstractControl): ValidationErrors | null {
  const stack: string[] = [];
  const re = /<\/?([a-z][a-z0-9]*)\b[^>]*>/gi;
  let m: RegExpExecArray | null;
  while ((m = re.exec(control.value ?? ''))) {
    const tag = m[1].toLowerCase();
    if (!ALLOWED_TAGS.has(tag)) return { forbiddenTag: tag };
    if (m[0].startsWith('</')) {
      if (stack.pop() !== tag) return { unclosedTag: true };
    } else {
      stack.push(tag);
    }
  }
  return stack.length ? { unclosedTag: true } : null;
}

@Component({
  selector: 'app-comment-form',
  imports: [ReactiveFormsModule, SafeCommentPipe, Captcha],
  templateUrl: './comment-form.html',
  styleUrl: './comment-form.css',
})
export class CommentForm {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly api = inject(CommentsService);

  /** Если задан — форма отправляет ответ на этот комментарий */
  readonly parentId = input<string | null>(null);
  readonly created = output<void>();

  private readonly textArea = viewChild.required<ElementRef<HTMLTextAreaElement>>('ta');
  private readonly captcha = viewChild(Captcha);

  protected readonly showPreview = signal(false);
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly form = this.fb.group({
    userName: ['', [Validators.required, Validators.pattern(/^[A-Za-z0-9]+$/)]],
    email: ['', [Validators.required, Validators.email]],
    homePage: ['', [Validators.pattern(/^(https?:\/\/)?([\w-]+\.)+[\w-]{2,}(\/\S*)?$/i)]],
    text: ['', [Validators.required, validTags]],
    captcha: [
      '',
      [
        Validators.required,
        (c: AbstractControl): ValidationErrors | null =>
          this.captcha()?.matches(c.value) ? null : { captchaMismatch: true },
      ],
    ],
  });

  protected invalid(name: FieldName): boolean {
    const c = this.form.controls[name];
    return c.invalid && (c.touched || c.dirty);
  }

  /** Картинка обновилась — старый ввод больше не актуален */
  protected onCaptchaRefreshed(): void {
    this.form.controls.captcha.reset();
  }

  /** Оборачивает выделенный текст в тег */
  protected wrap(tag: 'i' | 'strong' | 'code' | 'a'): void {
    const el = this.textArea().nativeElement;
    const { selectionStart: s, selectionEnd: e, value } = el;
    const open = tag === 'a' ? '<a href="" title="">' : `<${tag}>`;
    const wrapped = `${open}${value.slice(s, e)}</${tag}>`;
    this.form.controls.text.setValue(value.slice(0, s) + wrapped + value.slice(e));
    this.form.controls.text.markAsDirty();
    el.focus();
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { userName, email, homePage, text } = this.form.getRawValue();
    this.submitting.set(true);
    this.error.set(null);

    this.api
      .create({
        text,
        parentId: this.parentId(),
        user: { userName, email, homePage: homePage || null },
      })
      .subscribe({
        next: () => {
          this.form.controls.text.reset();
          this.captcha()?.refresh();
          this.showPreview.set(false);
          this.submitting.set(false);
          this.created.emit();
        },
        error: () => {
          this.error.set('Не удалось отправить комментарий. Проверьте, что сервер запущен, и повторите.');
          this.submitting.set(false);
        },
      });
  }
}
