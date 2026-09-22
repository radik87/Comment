import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { tap } from 'rxjs';
import { CommentDto, CreateCommentRequest } from '../models/comment.model';

/** Поменяйте на свой маршрут контроллера */
const API = 'https://localhost:44304/api/comment';

@Injectable({ providedIn: 'root' })
export class CommentsService {
  private readonly http = inject(HttpClient);
  private readonly flat = signal<CommentDto[]>([]);

  readonly loading = signal(false);
  readonly loadFailed = signal(false);

  /** Корневые комментарии с вложенными replies */
  readonly roots = computed(() => buildTree(this.flat()));

  load(): void {
    this.loading.set(true);
    this.loadFailed.set(false);
    this.http.get<CommentDto[]>(API).subscribe({
      next: list => {
        this.flat.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.loadFailed.set(true);
        this.loading.set(false);
      },
    });
  }

  create(body: CreateCommentRequest) {
    return this.http.post<CommentDto>(API, body).pipe(tap(() => this.load()));
  }
}

function buildTree(flat: CommentDto[]): CommentDto[] {
  const byId = new Map<string, CommentDto>();
  [...flat]
    .sort((a, b) => Date.parse(a.createdAt) - Date.parse(b.createdAt))
    .forEach(c => byId.set(c.id, { ...c, replies: [] }));

  const roots: CommentDto[] = [];
  for (const c of byId.values()) {
    const parent = c.parentId ? byId.get(c.parentId) : undefined;
    (parent ? parent.replies! : roots).push(c);
  }
  return roots;
}
