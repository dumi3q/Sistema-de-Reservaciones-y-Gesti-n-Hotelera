const fs=require('fs');
const original=fs.readFileSync('src/app/login/login.html','utf8');
const start=original.indexOf('      <div class="scene">');
const end=original.indexOf('      <div class="visual-copy">',start);
const scene=original.slice(start,end);
const html=`<aside class="visual-panel" aria-roledescription="carrusel" aria-label="Conoce HotelHub" (mouseenter)="hovered.set(true)" (mouseleave)="hovered.set(false)" (focusin)="focused.set(true)" (focusout)="onFocusOut($event)">
  @if (active() !== 1) {
    <img class="hotel-photo" [src]="active() === 0 ? '/images/hotel-exterior.webp' : '/images/hotel-garden.webp'" [alt]="active() === 0 ? 'Imagen conceptual de HotelHub: fachada crema, vegetación tropical y detalles verde bosque' : 'Imagen conceptual del jardín y piscina del hotel, con sillones verdes y arcos de piedra'" />
    <div class="photo-shade"></div>
  }
  <div class="visual-top"><span class="pill"><i></i> TU HOTEL, EN ARMONÍA</span><span class="edition">0{{active()+1}} — HOTELHUB</span></div>
  <div class="slide-content" [class.is-photo]="active() !== 1" aria-roledescription="diapositiva" [attr.aria-label]="'Vista '+(active()+1)+' de 3'">
    @if (active() === 1) {${scene}}
    @else { <div class="photo-space"><span class="photo-brand"><b>H</b> HOTELHUB</span><span class="photo-caption">IMAGEN CONCEPTUAL</span></div> }
    <div class="visual-copy"><p class="eyebrow">{{slides[active()].eyebrow}}</p><h2>{{slides[active()].title}}</h2><p>{{slides[active()].description}}</p></div>
  </div>
  <div class="visual-bottom"><div class="slide-controls" role="group" aria-label="Elegir imagen">@for(slide of slides;track $index){<button type="button" [class.slide-active]="active()===$index" [attr.aria-label]="'Mostrar imagen '+($index+1)+': '+slide.label" [attr.aria-pressed]="active()===$index" (click)="select($index)"><span></span></button>}</div><button type="button" class="pause-control" (click)="paused.set(!paused())" [attr.aria-label]="paused()?'Reanudar carrusel':'Pausar carrusel'">{{paused()?'▶':'Ⅱ'}}</button></div>
</aside>`;
fs.writeFileSync('src/app/shared/auth-carousel/auth-carousel.html',html);
fs.copyFileSync('src/app/login/login-visual.css','src/app/shared/auth-carousel/auth-carousel.css');
