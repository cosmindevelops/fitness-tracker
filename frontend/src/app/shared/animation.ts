import { animate, style, transition, trigger } from '@angular/animations';

export const slideDownAnimation = trigger('slideDown', [
  transition(':enter', [style({ opacity: 0, transform: 'translateY(-100px)' }), animate('0.6s ease', style({ opacity: 1, transform: 'translateY(0)' }))]),
]);

export const slideDownWithBlurAnimation =
  trigger('slideDownWithBlur', [
    transition(':enter', [
      style({ opacity: 0, transform: 'translateY(-100px)', backdropFilter: 'blur(15px)' }),
      animate('0.6s ease', style({ opacity: 1, transform: 'translateY(0)', backdropFilter: 'blur(15px)' })),
    ]),
  ]);