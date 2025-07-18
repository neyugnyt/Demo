/***************************************************************************************************
 * Load `$localize` onto the global scope - used if i18n tags appear in Angular templates.
 */
import '@angular/localize/init';
import 'zone.js/node';

// Refrence Error Event Not Found Solution
global['Event'] = null;

// Refrence Error Window Not found  solution
const domino = require('domino');
const fs = require('fs');
const path = require('path');
const template = fs.readFileSync(path.join(__dirname, '../', '', 'browser/index.html')).toString();
const win = domino.createWindow(template);
global['window'] = win;
global['document'] = win.document;
global['navigator'] = win.navigator;

// Refrence Error localStorage Not found  solution
import 'localstorage-polyfill'
global['localStorage'] = localStorage;

const cors = require('cors');

import { enableProdMode } from '@angular/core';

import { ngExpressEngine } from '@nguniversal/express-engine';
import * as express from 'express';
import { join } from 'path';

import { AppServerModule } from './src/main.server';
import { APP_BASE_HREF } from '@angular/common';
import { existsSync } from 'fs';
import { provideModuleMap } from '@nguniversal/module-map-ngfactory-loader';

// Faster server renders w/ Prod mode (dev mode never needed)
enableProdMode();

// The Express app is exported so that it can be used by serverless Functions.
export function app() {
  const server = express();
  server.use(cors());
  const distFolder = join(process.cwd(), 'dist/multikart/browser');
  const indexHtml = existsSync(join(distFolder, 'index.original.html')) ? 'index.original.html' : 'index';
  const LAZY_MODULE_MAP = require('./src/main.server');
  // Our Universal express-engine (found @ https://github.com/angular/universal/tree/master/modules/express-engine)
  server.engine('html', ngExpressEngine({
    bootstrap: AppServerModule,
    providers: [
      provideModuleMap(LAZY_MODULE_MAP)
    ]
  }));

  server.set('view engine', 'html');
  server.set('views', distFolder);

  
  // Example Express Rest API endpoints
  // app.get('/api/**', (req, res) => { });
  // Serve static files from /browser
  server.get('*.*', express.static(distFolder, {
    maxAge: '1y'
  }));

  // All regular routes use the Universal engine
  server.get('*', (req, res) => {
    res.render(indexHtml, { req, 
      providers: [{ 
        provide: APP_BASE_HREF,
        useValue: req.baseUrl 
      }]
    });
  });

  return server;
}

function run() {
  const port = process.env.PORT || 4000;

  // Start up the Node server
  const server = app();
  server.listen(port, () => {
    // console.log(`Node Express server listening on http://localhost:${port}`);
  });
}

// Webpack will replace 'require' with '__webpack_require__'
// '__non_webpack_require__' is a proxy to Node 'require'
// The below code is to ensure that the server is run only when not requiring the bundle.
declare const __non_webpack_require__: NodeRequire;
const mainModule = __non_webpack_require__.main;
const moduleFilename = mainModule && mainModule.filename || '';
if (moduleFilename === __filename || moduleFilename.includes('iisnode')) {
  run();
}

export * from './src/main.server';
export { renderModule, renderModuleFactory } from '@angular/platform-server';