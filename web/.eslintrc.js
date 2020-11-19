module.exports = {
  root: true,
  env: {
    browser: true,
    node: true
  },
  extends: [
    '@nuxtjs/eslint-config-typescript',
    'plugin:nuxt/recommended'
  ],
  plugins: [
  ],
  // add your custom rules here
  rules: {
    'space-before-function-paren': ['error', 'never'],
    'keyword-spacing': ['error', {
      overrides: {
        if: { after: false },
        for: { after: false },
        while: { after: false }
      }
    }],
    'nuxt/no-cjs-in-config': 'off',
    'vue/singleline-html-element-content-newline': 'off',
    'vue/multiline-html-element-content-newline': 'off',
    'vue/html-self-closing': 'off',
    'no-console': 'off',
    'v-html': 'off',
    'vue/no-v-html': 'off',
    'no-debugger': 'off',
    'vue/no-use-v-if-with-v-for': 'off',
    'vue/max-attributes-per-line': 'off',
    'vue/valid-v-slot': 'off'
  }
}
