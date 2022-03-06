var terser = require('terser-webpack-plugin')
const path = require('path')

module.exports = {
    mode: 'development',
    entry: {
        entry: './index.ts'
    },
    output: {
        path: path.resolve(__dirname, '../wwwroot/font'),
        filename: 'font.js',
        library: 'porphystructFonts'
    },
    resolve: {
        alias: {
            components: path.resolve(__dirname, '/'),
        },
        extensions: ['.ts', '.js'],
    },
    module: {
        rules: [
            { test: /\.js$/, use: 'babel-loader' },
            { test: /\.ts$/, use: 'ts-loader' },
            { test: /\.css$/, use: ['style-loader', 'css-loader']},
            { test: /\.scss$/, use: 'sass-loader'},
            { test: /\.(woff|woff2|eot|ttf|otf)$/i, type: 'asset/resource' }
        ]
    },
    optimization: {
        usedExports: false
    },
    plugins: [
        new terser()
    ]
}