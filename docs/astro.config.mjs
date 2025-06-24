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
					label: 'Guides',
					items: [
						// Each item here is one entry in the navigation menu.
						{ label: 'Example Guide', slug: 'guides/example' },
					],
				},
				{
					label: 'Reference',
					autogenerate: { directory: 'reference' },
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