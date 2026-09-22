export interface UserDto {
  id?: string;
  userName: string;
  email: string;
  homePage?: string | null;
}

export interface CommentDto {
  id: string;
  text: string;
  createdAt: string;
  filePath: string | null;
  fileType: string | null;
  userId: string;
  /** Нужно добавить на бэкенде — без него ответы на комментарии невозможны */
  parentId?: string | null;
  user: UserDto;
  /** Заполняется на клиенте при построении дерева */
  replies?: CommentDto[];
}

export interface CreateCommentRequest {
  text: string;
  parentId?: string | null;
  user: Pick<UserDto, 'userName' | 'email' | 'homePage'>;
}
