import express from 'express';
import { initBestTrackers } from './lib/magnetHelper.js';
import {processConfig} from "./lib/settings.js";
import serverless from './serverless.js';

const app = express();
app.enable('trust proxy');
app.use(express.static('static', { maxAge: '1y' }));
app.use((req, res) => serverless(req, res));
app.listen(processConfig.PORT, () => {
  initBestTrackers()
      .then(() => console.log(`Started addon at: http://localhost:${processConfig.PORT}`));
});
