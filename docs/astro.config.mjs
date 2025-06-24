// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import sitemap from '@astrojs/sitemap';

import tailwindcss from '@tailwindcss/vite';

// https://astro.build/config
export default defineConfig({
	site: 'https://porphystruct.org',
	integrations: [
		starlight({
			title: 'PorphyStruct',
			customCss: [
				'@fontsource-variable/roboto',
				'./src/styles/global.css',
			],
			social: [{ icon: 'github', label: 'GitHub', href: 'https://github.com/jenskrumsieck/porphystruct' }],
			logo: {
				src: './src/assets/logo.svg'
			},
			sidebar: [
				{
					label: 'Home',
					slug: '',
				},
				{	
					label: 'Documentation',
					items: [
						{ label: 'Example Guide', slug: 'docs/example' },
					],
				},
			],
			components: {
				Hero: './src/components/Hero.astro',
				Footer: './src/components/Footer.astro',
			}
		}),
		sitemap()
	],

	vite: {
		plugins: [tailwindcss()],
	},
});