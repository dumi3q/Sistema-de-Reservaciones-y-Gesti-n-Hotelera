const { chromium } = require('C:/Users/evans/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/playwright');
(async()=>{
 const browser=await chromium.launch({headless:true,channel:'msedge'});
 try {
  const page=await browser.newPage({viewport:{width:1440,height:1000}});const errors=[];page.on('pageerror',e=>errors.push(e.message));
  await page.goto('http://127.0.0.1:64861/');await page.waitForURL('**/visitar');await page.getByRole('heading',{name:/Hay lugares/}).waitFor();
  if(await page.locator('nav a[href="/visitar"]').count()!==1)throw Error('Public home navigation missing');
  if(await page.locator('nav a[href="/explorar/restaurante"]').count())throw Error('Operational restaurant exposed in guest navigation');
  await page.screenshot({path:'work/ui-review/public-home.png',fullPage:true});
  await page.goto('http://127.0.0.1:64861/login');await page.locator('.hotel-photo').waitFor();
  await page.mouse.move(5,5);await page.waitForTimeout(6750);
  if(await page.locator('.mini-dashboard').count()!==1)throw Error('Carousel did not automatically advance');
  await page.getByRole('button',{name:/Mostrar imagen 3/}).click();
  await page.waitForFunction(()=>document.querySelector('.hotel-photo')?.getAttribute('src').includes('garden'));
  await page.getByRole('button',{name:/Mostrar imagen 1/}).click();
  await page.screenshot({path:'work/ui-review/login-carousel.png',fullPage:true});
  await page.getByRole('link',{name:'Crear cuenta',exact:true}).click();await page.waitForURL('**/registro');
  await page.getByLabel('Nombres',{exact:true}).fill('Ana');await page.getByLabel('Apellidos',{exact:true}).fill('Mora');
  await page.getByLabel('Correo electrónico',{exact:true}).fill('ana@example.com');
  await page.getByLabel('Contraseña',{exact:true}).fill('example-pass');await page.getByLabel('Confirmar contraseña',{exact:true}).fill('different-pass');
  await page.getByRole('button',{name:'Crear cuenta de huésped'}).click();await page.getByText('Las contraseñas deben coincidir.').waitFor();
  await page.getByRole('link',{name:'Continuar como invitado'}).click();await page.waitForURL('**/visitar');
  await page.getByRole('link',{name:'Encuentra tu habitación'}).click();await page.locator('.room-card').first().waitFor();
  await page.locator('.room-card').first().click();await page.getByRole('button',{name:'Elegir esta habitación'}).click();
  await page.waitForURL('**/login?**');await page.getByText(/Tu habitación te espera/).waitFor();
  if(!page.url().includes('habitacion=A-101'))throw Error('Room selection context lost');
  await page.getByRole('link',{name:'Crear cuenta',exact:true}).click();await page.waitForURL('**/registro?**');
  if(!page.url().includes('habitacion=A-101'))throw Error('Registration context lost');
  await page.goto('http://127.0.0.1:64861/explorar/inicio');await page.locator('table tbody tr').first().waitFor();
  await page.getByRole('button',{name:'2026-12-11, con movimientos',exact:true}).click();
  await page.waitForFunction(()=>document.querySelectorAll('table tbody tr').length===2);
  await page.getByRole('button',{name:'Volver a la fecha de ejemplo'}).click();
  await page.waitForFunction(()=>document.querySelectorAll('table tbody tr').length===4);
  await page.screenshot({path:'work/ui-review/dashboard.png',fullPage:true});
  const overflow=[];
  for(const route of ['/login','/registro','/visitar','/visitar/habitaciones','/visitar/restaurante','/explorar/inicio']){
   await page.setViewportSize({width:390,height:844});await page.goto('http://127.0.0.1:64861'+route);await page.waitForTimeout(800);
   if(await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth))overflow.push(route);
   if(route==='/explorar/inicio')await page.screenshot({path:'work/ui-review/dashboard-mobile.png',fullPage:true});
  }
  console.log(JSON.stringify({errors,overflow,flows:'public browsing, reservation login gate, registration validation, calendar and carousel checked'}));
  if(errors.length||overflow.length)process.exitCode=1;
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exit(1)});
