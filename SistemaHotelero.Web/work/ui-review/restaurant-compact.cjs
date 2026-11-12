const {chromium}=require('C:/Users/evans/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/playwright');
(async()=>{
 const browser=await chromium.launch({headless:true,channel:'msedge'});
 try {
  const page=await browser.newPage();const errors=[];page.on('pageerror',e=>errors.push(e.message));
  const checks=[];
  for(const size of [{width:1366,height:768},{width:1280,height:720},{width:1440,height:900}]){
   await page.setViewportSize(size);await page.goto('http://127.0.0.1:64861/explorar/restaurante');
   await page.locator('.table-tile').first().waitFor();
   const check=await page.evaluate(()=>({pageHeight:document.documentElement.scrollHeight,viewport:innerHeight,mapBottom:document.querySelector('.floor-plan').getBoundingClientRect().bottom,allTablesVisible:[...document.querySelectorAll('.table-tile')].every(t=>t.getBoundingClientRect().bottom<=innerHeight),horizontalOverflow:document.documentElement.scrollWidth>innerWidth}));
   checks.push({...size,...check});
   if(size.width===1366)await page.screenshot({path:'work/ui-review/restaurant-compact.png'});
  }
  await page.getByRole('button',{name:/Por tomar orden:.*Filtrar/}).click();
  await page.waitForFunction(()=>document.querySelectorAll('.table-tile').length===2);
  await page.getByRole('button',{name:/Mesa R2,/}).click();
  await page.getByRole('dialog').waitFor();
  await page.getByRole('button',{name:'Simular toma de orden'}).click();
  await page.getByRole('dialog').getByText('Cambio de demostración',{exact:false}).waitFor();
  await page.getByRole('button',{name:'Cerrar detalle de mesa'}).click();
  await page.setViewportSize({width:390,height:844});
  await page.getByRole('button',{name:'Piscina',exact:true}).click();
  await page.locator('.table-tile').first().waitFor();
  await page.screenshot({path:'work/ui-review/restaurant-compact-mobile.png',fullPage:true});
  const mobileOverflow=await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth);
  console.log(JSON.stringify({checks,errors,mobileOverflow}));
  if(checks.some(c=>!c.allTablesVisible||c.horizontalOverflow)||errors.length||mobileOverflow)process.exitCode=1;
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exit(1)});
