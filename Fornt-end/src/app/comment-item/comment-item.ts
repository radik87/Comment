import { DatePipe } from '@angular/common';
import { Component, computed, input, signal } from '@angular/core';
import { CommentDto } from '../models/comment.model';
import { SafeCommentPipe } from '../pipes/safe-comment.pipe';
import { CommentForm } from '../comment-form/comment-form';

/** Один комментарий + форма ответа + рекурсивно все ответы на него */
@Component({
  selector: 'app-comment-item',
  imports: [DatePipe, SafeCommentPipe, CommentForm],
  templateUrl: './comment-item.html',
  styleUrl: './comment-item.css',
})
export class CommentItem {
  readonly comment = input.required<CommentDto>();
  /** В таблице автор и дата уже есть в колонках, поэтому шапку скрываем */
  readonly showHeader = input(true);
  protected readonly replying = signal(false);

  protected readonly homeHref = computed(() => {
    const h = this.comment().user.homePage?.trim();
    if (!h) return null;
    return /^https?:\/\//i.test(h) ? h : `https://${h}`;
  });
}
