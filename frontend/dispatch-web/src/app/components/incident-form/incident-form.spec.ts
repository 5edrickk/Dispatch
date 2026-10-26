import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IncidentForm } from './incident-form';

describe('IncidentForm', () => {
  let component: IncidentForm;
  let fixture: ComponentFixture<IncidentForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IncidentForm],
    }).compileComponents();

    fixture = TestBed.createComponent(IncidentForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('updates the form when an option tile is picked', () => {
    const radioFor = (text: string): HTMLInputElement =>
      [...fixture.nativeElement.querySelectorAll('label.option')]
        .find((l: HTMLElement) => l.textContent?.trim() === text)
        .querySelector('input');

    radioFor('Police').click();
    radioFor('Critical').click();

    expect(component.form.controls.type.value).toBe('Police');
    expect(component.form.controls.priority.value).toBe('Critical');
  });
});
