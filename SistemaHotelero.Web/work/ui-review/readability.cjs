const fs = require('fs');
const files = [
  'src/app/shared/explorer.css',
  'src/app/layout/layout.css',
  'src/app/reservar/reservar.css',
  'src/app/reservar/room-detail.css',
  'src/app/restaurante/restaurante.css',
];
for (const file of files) {
  let css = fs.readFileSync(file, 'utf8');
  css = css.replace(/font-size:\s*(\d+)px/g, (_, size) => {
    const value = Number(size);
    const larger = value <= 11 ? 13 : value <= 13 ? 15 : value <= 14 ? 16 : value <= 16 ? 18 : value;
    return `font-size: ${larger}px`;
  });
  if (!file.includes('layout/')) {
    css = css.replace(/(^\s*color:\s*)(#[\da-f]{6})(;)/gmi, (match, prefix, hex, suffix) => {
      const values = [1,3,5].map(start => parseInt(hex.slice(start,start+2),16));
      const brightness = .2126*values[0] + .7152*values[1] + .0722*values[2];
      return brightness > 112 && brightness < 220 ? `${prefix}#5b6955${suffix}` : match;
    });
  }
  css = css.replace(/font-weight:\s*450/g, 'font-weight: 500');
  fs.writeFileSync(file, css);
}
let layout=fs.readFileSync('src/app/layout/layout.css','utf8');
layout=layout.replace('220px minmax(0, 1fr)','240px minmax(0, 1fr)').replace('180px minmax(0, 1fr)','220px minmax(0, 1fr)').replace('@media (max-width: 760px)','@media (max-width: 850px)');
layout=layout.replaceAll('#7e8b7c','#53634e').replaceAll('#89947f','#53634e').replaceAll('#657458','#53634e');
layout+='\nnav a { min-height: 48px; line-height: 1.5; }\n.topbar { flex-wrap: wrap; }\n.workspace-footer { gap: 16px; line-height: 1.6; }\n.workspace-content { line-height: 1.5; }\n@media (max-width: 850px) { nav a { min-width: 0; } .nav-arrow { display: none; } .workspace-footer { align-items: start; } }\n';
fs.writeFileSync('src/app/layout/layout.css',layout);
let shared=fs.readFileSync('src/app/shared/explorer.css','utf8');
shared+='\n.page { font-size: 16px; line-height: 1.6; }\n.button-primary { min-height: 48px; font-weight: 600; }\n.section-heading { flex-wrap: wrap; }\n.section-heading h2 { font-size: 20px; }\n.step { width: 32px; height: 32px; flex-shrink: 0; }\n.section-meta { text-align: left; }\n';
fs.writeFileSync('src/app/shared/explorer.css',shared);
let rooms=fs.readFileSync('src/app/reservar/reservar.css','utf8');
rooms+='\n.building-description { flex-wrap: wrap; gap: 10px; }\n.building-description h3 { font-size: 18px; }\n.building-description div > span { display: block; line-height: 1.6; }\n.building-tagline { line-height: 1.6; margin: 12px 5px; }\n.availability { padding: 6px 8px; }\n.search-panel input, .search-panel select { min-height: 44px; font-size: 16px; }\n.search-panel .button-primary { flex-shrink: 0; }\n.floor-button small { white-space: normal; line-height: 1.6; }\n.room-state { line-height: 1.5; }\n.room-card { gap: 12px; padding: 20px 16px; }\n.room-card > small { line-height: 1.6; }\n.room-panel-heading { flex-wrap: wrap; line-height: 1.6; }\n.hallway { font-size: 11px; letter-spacing: 0; flex-wrap: wrap; }\n.room-legend span:last-child { font-size: 13px; }\n@media (min-width: 761px) and (max-width: 1450px) { .building-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); } .building-description { display: flex; } }\n@media (max-width: 760px) { .building-description { display: block; } .availability { margin-top: 10px; } .building-tagline { display: none; } .floor-button { padding: 12px 9px; } .search-panel .button-primary { flex-basis: 100%; font-size: 15px; } }\n';
fs.writeFileSync('src/app/reservar/reservar.css',rooms);
let detail=fs.readFileSync('src/app/reservar/room-detail.css','utf8');
detail+='\n.detail-content { padding: 23px; }\n.detail-summary { font-size: 15px; }\n.detail-content h3 { font-size: 25px; }\n.detail-content h4 { font-size: 16px; }\n.photo-tabs button { width: 44px; height: 40px; }\n.price-row, .estimate { flex-wrap: wrap; gap: 10px; }\n.room-facts { gap: 12px; line-height: 1.6; }\n.amenities span { padding: 8px 10px; }\n.price-note { font-size: 13px; }\n';
fs.writeFileSync('src/app/reservar/room-detail.css',detail);
let restaurant=fs.readFileSync('src/app/restaurante/restaurante.css','utf8');
restaurant=restaurant.replaceAll('#728c5d','#4c693b').replaceAll('#b1853d','#805b1e').replaceAll('#668a9c','#3d6578').replaceAll('#7e7d9d','#60577d').replaceAll('#ad7663','#885340');
restaurant+='\n.area-tabs { flex-wrap: wrap; }\n.area-tabs button, .state-filter select { min-height: 44px; }\n.table-detail-top > div { flex-wrap: wrap; }\n.table-status { flex-wrap: wrap; justify-content: center; text-align: center; }\n.table-extra { line-height: 1.6; text-align: center; }\n.map-area header, .map-top { flex-wrap: wrap; gap: 10px; }\n.legend-item { line-height: 1.6; }\n.table-surface small { opacity: 1; }\n@media (max-width: 1150px) { .tables-grid { grid-template-columns: repeat(2,minmax(0,1fr)); } }\n@media (max-width: 760px) { .area-tabs button { flex: 1 1 40%; } .table-shape { width: 90px; } .round { width: 80px; } .service-summary article { min-width: 0; } .service-summary small { line-height: 1.6; } }\n';
// Keep the map and detail styles in separate readable files.
const split=restaurant.indexOf('.table-detail {');
fs.writeFileSync('src/app/restaurante/restaurante.css', restaurant.slice(0,split));
fs.writeFileSync('src/app/restaurante/table-detail.css', restaurant.slice(split));
let ts=fs.readFileSync('src/app/restaurante/restaurante.ts','utf8');
ts=ts.replace("'./restaurante.css'", "'./restaurante.css', './table-detail.css'");
fs.writeFileSync('src/app/restaurante/restaurante.ts',ts);
let global=fs.readFileSync('src/styles.css','utf8').replace('--muted: #849084','--muted: #5b6955');
global=global.replace('  min-width: 320px;', '  min-width: 320px;\n  font-family: var(--font-ui);\n  font-size: 16px;\n  line-height: 1.5;');
fs.writeFileSync('src/styles.css',global);
