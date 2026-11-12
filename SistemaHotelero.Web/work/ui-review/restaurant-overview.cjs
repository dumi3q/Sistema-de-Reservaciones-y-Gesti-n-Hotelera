const fs=require('fs');
const path='src/app/restaurante/restaurante.html';
let html=fs.readFileSync(path,'utf8');
const headerEnd=html.indexOf('  <div class="restaurant-filters">');
html=`<section class="page restaurant-page" aria-labelledby="restaurant-title" [attr.aria-busy]="loading()">
  <header class="page-heading">
    <div><p class="eyebrow">RESTAURANTE · PISCINA · BAR</p><h1 id="restaurant-title">Todo el salón, de un vistazo.</h1></div>
    <span class="service-total">{{ tables().length }} mesas · {{ people() }} comensales <small>Vista de prueba</small></span>
  </header>
  <div class="attention-summary" role="group" aria-label="Estado de todas las mesas del hotel">
    @for (status of states; track status) {
      <button type="button" [class]="'attention-card ' + tone(status)" [class.active]="statusFilter() === status" [attr.aria-pressed]="statusFilter() === status" [attr.aria-label]="status + ': ' + count(status) + ' mesas. Filtrar en todas las áreas'" (click)="showStatus(status)" [disabled]="loading()">
        @if (loading()) { <app-skeleton [height]="38" /> } @else { <strong>{{ count(status).toString().padStart(2, '0') }}</strong> }
        <span>{{ status }}<small>{{ status === 'Libre' ? 'Listas para recibir' : status === 'Por tomar orden' ? 'Necesitan atención' : status === 'Por cobrar' ? 'Cuenta pendiente' : status === 'En preparación' ? 'Pedido en cocina' : 'Servicio entregado' }}</small></span>
        <i aria-hidden="true"></i>
      </button>
    }
  </div>
  <p class="screen-reader-only" role="status">{{ count('Libre') }} mesas libres, {{ count('Por tomar orden') }} por tomar orden y {{ count('Por cobrar') }} por cobrar.</p>
`+html.slice(headerEnd);
const legendStart=html.indexOf('  <div class="status-legend">');
const legendEnd=html.indexOf('  @if (error())',legendStart);
html=html.slice(0,legendStart)+html.slice(legendEnd);
html=html.replace('        @for (section of visibleAreas(); track section) {','        <div class="areas-grid">\n        @for (section of visibleAreas(); track section) {');
html=html.replace('      }\n      <div class="map-bottom">','        </div>\n      }\n      <div class="map-bottom">');
html=html.replace('<aside class="table-detail" aria-live="polite">','<dialog #detailDialog class="table-dialog" aria-labelledby="table-detail-title" (close)="selectedId.set(null)">\n<button type="button" class="close-detail" aria-label="Cerrar detalle de mesa" (click)="detailDialog.close()">✕</button>\n<aside class="table-detail" aria-live="polite">');
html=html.replace('<h3>Mesa {{ table.id }}</h3>','<h3 id="table-detail-title">Mesa {{ table.id }}</h3>');
html=html.replace('    </aside>','    </aside>\n</dialog>');
fs.writeFileSync(path,html);
let ts=fs.readFileSync('src/app/restaurante/restaurante.ts','utf8');
ts=ts.replace('Component, computed, inject, OnInit, signal','Component, computed, ElementRef, inject, OnInit, signal, viewChild');
ts=ts.replace("'./table-detail.css'", "'./table-detail.css', './restaurant-dialog.css'");
ts=ts.replace("signal<string | null>('R2')","signal<string | null>(null)");
ts=ts.replace('  private readonly demo',"  readonly detailDialog = viewChild<ElementRef<HTMLDialogElement>>('detailDialog');\n  private readonly demo");
ts=ts.replace('    this.selectedId.set(table.id);','    this.selectedId.set(table.id);\n    this.detailDialog()?.nativeElement.showModal?.();');
ts=ts.replace('  filterStatus(status: string)',"  showStatus(status: TableStatus) {\n    const next = this.statusFilter() === status ? 'Todos' : status;\n    this.area.set('Todas las áreas');\n    this.filterStatus(next);\n  }\n  filterStatus(status: string)");
fs.writeFileSync('src/app/restaurante/restaurante.ts',ts);
let layout=fs.readFileSync('src/app/layout/layout.ts','utf8');
layout=layout.replace("import { Component } from '@angular/core';", "import { Component, inject } from '@angular/core';\nimport { toSignal } from '@angular/core/rxjs-interop';\nimport { filter, map } from 'rxjs';");
layout=layout.replace('RouterLink, RouterLinkActive, RouterOutlet','NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet');
layout=layout.replace('export class Layout {}',`export class Layout {
 private readonly router = inject(Router);
 readonly restaurantMode = toSignal(this.router.events.pipe(
   filter((event): event is NavigationEnd => event instanceof NavigationEnd),
   map((event) => event.urlAfterRedirects.includes('/restaurante')),
 ), { initialValue: this.router.url.includes('/restaurante') });
}`);
fs.writeFileSync('src/app/layout/layout.ts',layout);
let template=fs.readFileSync('src/app/layout/layout.html','utf8').replace('<div class="workspace">','<div class="workspace" [class.restaurant-mode]="restaurantMode()">');
fs.writeFileSync('src/app/layout/layout.html',template);
let detail=fs.readFileSync('src/app/restaurante/table-detail.css','utf8');
const media=detail.indexOf('@media');
if(media!==-1)detail=detail.slice(0,media);
fs.writeFileSync('src/app/restaurante/table-detail.css',detail);
