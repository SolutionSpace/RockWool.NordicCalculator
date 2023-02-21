const path = require('path');

const FixStyleOnlyEntriesPlugin = require("webpack-fix-style-only-entries");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');

// settings
const nodeModulesFolder = "./node_modules";
const cssFolder = "./Content/css";
const bundlesFolder = "./bundles";
const cssBundlePath = 'plugins.css';
const scriptPart = "script-loader!";

module.exports = {
    mode: "production",
    entry: {
        // js
        "jquery": [
            `${scriptPart}${nodeModulesFolder}/jquery/dist/jquery.js`,
            `${scriptPart}${nodeModulesFolder}/jquery-validation/dist/jquery.validate.min.js`,
            `${scriptPart}${nodeModulesFolder}/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js`,
            `${scriptPart}${nodeModulesFolder}/jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax.min.js`
        ],
        "plugins": [
            `${scriptPart}${nodeModulesFolder}/popper.js/dist/umd/popper.min.js`,
            `${scriptPart}${nodeModulesFolder}/bootstrap-material-design/dist/js/bootstrap-material-design.min.js`,
            `${scriptPart}${nodeModulesFolder}/bootstrap-select/js/bootstrap-select.js`,
            `${scriptPart}${nodeModulesFolder}/browser-detect/dist/browser-detect.umd.js`
        ],
        // css
        cssBundlePath: [
            `${cssFolder}/bootstrap-material-design.css`,
            `${cssFolder}/bootstrap-select.css`,
            `${cssFolder}/paged-list-mvc.css`
        ]
    },
    output: {
        filename: "[name].js",
        path: path.resolve(__dirname, `${bundlesFolder}`)
    },
    module: {
        rules: [
            {
                test: /\.css$/,
                use: [
                    {
                        loader: MiniCssExtractPlugin.loader,
                        options: {
                            minimize: true
                        }
                    },
                    'css-loader'
                ]
            }
        ]
    },
    plugins: [
        new FixStyleOnlyEntriesPlugin(),
        new MiniCssExtractPlugin({
            filename: cssBundlePath
        }),
        new OptimizeCssAssetsPlugin({})
    ]
}