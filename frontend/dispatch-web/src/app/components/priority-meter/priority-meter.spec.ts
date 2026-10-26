import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PriorityMeter } from './priority-meter';

describe('PriorityMeter', () => {
  let fixture: ComponentFixture<PriorityMeter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PriorityMeter],
    }).compileComponents();

    fixture = TestBed.createComponent(PriorityMeter);
  });

  it('fills bars up to the priority level', async () => {
    fixture.componentRef.setInput('priority', 'High');
    await fixture.whenStable();

    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelectorAll('.bar').length).toBe(4);
    expect(el.querySelectorAll('.bar.filled').length).toBe(3);
  });
});
