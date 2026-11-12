const fs=require('fs');
let ts=fs.readFileSync('src/app/reservar/reservar.ts','utf8');
ts="import { Router } from '@angular/router';\n"+ts;
ts=ts.replace('  readonly demo = inject(HotelDemoService);',"  private readonly router = inject(Router);\n  readonly publicMode = this.router.url.startsWith('/visitar');\n  readonly demo = inject(HotelDemoService);");
ts=ts.replace('    if (!this.available(room)) return;',`    if (!this.available(room)) return;
    if (this.publicMode) {
      void this.router.navigate(['/login'], { queryParams: {
        returnUrl:'/visitar/habitaciones', habitacion:room.id,
        entrada:this.dates().start, salida:this.dates().end, adultos:this.dates().guests,
      }});
      return;
    }`);
fs.writeFileSync('src/app/reservar/reservar.ts',ts);
let spec=fs.readFileSync('src/app/reservar/reservar.spec.ts','utf8');
spec="import { provideRouter } from '@angular/router';\n"+spec;
spec=spec.replace('imports: [Reservar]', 'imports: [Reservar], providers: [provideRouter([])]');
fs.writeFileSync('src/app/reservar/reservar.spec.ts',spec);
