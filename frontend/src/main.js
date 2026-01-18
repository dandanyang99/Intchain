import {
	createSSRApp
} from "vue";
import App from "./App.vue";
import uviewPlus from 'uview-plus';
import pinia from './store/index.js';

export function createApp() {
	const app = createSSRApp(App);
	app.use(pinia);
	app.use(uviewPlus);
	return {
		app,
	};
}
