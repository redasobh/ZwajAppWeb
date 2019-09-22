import { TestBed, async, inject } from '@angular/core/testing';

import { ChargeGuard } from './charge.guard';

describe('ChargeGuard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ChargeGuard]
    });
  });

  it('should ...', inject([ChargeGuard], (guard: ChargeGuard) => {
    expect(guard).toBeTruthy();
  }));
});
