import { TestBed, async, inject } from '@angular/core/testing';

import { MessagesGuard } from './messages.guard';

describe('MessagesGuard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MessagesGuard]
    });
  });

  it('should ...', inject([MessagesGuard], (guard: MessagesGuard) => {
    expect(guard).toBeTruthy();
  }));
});
