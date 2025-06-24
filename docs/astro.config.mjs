// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import sitemap from '@astrojs/sitemap';
import tailwindcss from '@tailwindcss/vite';
import starlightSidebarTopics from 'starlight-sidebar-topics'
import starlightImageZoom from 'starlight-image-zoom'
import remarkMath from 'remark-math';
import rehypeMathJax from 'rehype-mathjax';

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
			components: {
				Hero: './src/components/Hero.astro',
				Footer: './src/components/Footer.astro',
			},
			plugins: [
				starlightImageZoom(),
				starlightSidebarTopics([
					{
						label: 'Home',
						icon: 'puzzle',
						link: '/',
					},
					{
						label: 'Documentation',
						icon: 'open-book',
						link: '/docs/getting-started',
						items: [
							{ label: 'Getting Started', autogenerate: { directory: 'docs/getting-started' } },
							{ label: 'Concepts', autogenerate: { directory: 'docs/concepts' } },
							{ label: 'Analysis', autogenerate: { directory: 'docs/analysis' } },
							{ label: 'Advanced Topics', autogenerate: { directory: 'docs/advanced' } },
							{ label: 'Settings', autogenerate: { directory: 'docs/settings' } },
						]
					},
					{
						label: 'Download for Windows',
						icon: 'seti:windows',
						link: 'https://github.com/JensKrumsieck/PorphyStruct/releases/latest/',
					},
					{
						label: 'Web Application',
						icon: 'cloud-download',
						link: 'https://app.porphystruct.org/',
					},
				]),
			],
		}),
		sitemap(),
	],
	markdown: {
		remarkPlugins: [remarkMath],
		rehypePlugins: [rehypeMathJax],
	},
	vite: {
		plugins: [tailwindcss()],
	},
});