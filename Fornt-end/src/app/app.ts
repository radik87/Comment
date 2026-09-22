import { Component } from '@angular/core';
import { CommentForm } from './comment-form/comment-form';
import { CommentList } from './comment-list/comment-list';

@Component({
  selector: 'app-root',
  imports: [CommentForm, CommentList],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {}
