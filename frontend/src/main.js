// Dark inicial ANTES de montar (usa clase .p-dark en <html>)
const isDark = localStorage.getItem('dark') === '1';
document.documentElement.classList.toggle('p-dark', isDark);

import { createApp } from 'vue'
import App from './App.vue'

import PrimeVue from 'primevue/config'
import Lara from '@primevue/themes/lara'
import ToastService from 'primevue/toastservice'
import ConfirmationService from 'primevue/confirmationservice'
import 'primeicons/primeicons.css'

import Button from 'primevue/button'
import Card from 'primevue/card'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
import Dropdown from 'primevue/dropdown'
import InputSwitch from 'primevue/inputswitch'
import InlineMessage from 'primevue/inlinemessage'
import Tag from 'primevue/tag'
import Toast from 'primevue/toast'
import ConfirmDialog from 'primevue/confirmdialog'
import Checkbox from 'primevue/checkbox'



const app = createApp(App)

app.use(PrimeVue, {
  theme: {
    preset: Lara,
    options: {
      darkModeSelector: '.p-dark',  
      inputVariant: 'filled'
    }
  }
})

app.use(ToastService)
app.use(ConfirmationService)

app.component('Button', Button)
app.component('Card', Card)
app.component('DataTable', DataTable)
app.component('Column', Column)
app.component('InputText', InputText)
app.component('Dropdown', Dropdown)
app.component('Checkbox', Checkbox)
app.component('InlineMessage', InlineMessage)
app.component('Tag', Tag)
app.component('Toast', Toast)
app.component('ConfirmDialog', ConfirmDialog)

app.mount('#app')
