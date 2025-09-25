<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import axios from 'axios'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'

const api = axios.create({ baseURL: 'http://localhost:5099/api' })

const toast = useToast()
const confirm = useConfirm()

// ------- UI (dark + 1 toast a la vez) -------
const dark = ref(document.documentElement.classList.contains('p-dark'))
watch(dark, v => {
  document.documentElement.classList.toggle('p-dark', v)
  localStorage.setItem('dark', v ? '1' : '0')
})
const notify = (severity, summary, detail = '', life = 2200) => {
  try { toast.removeGroup && toast.removeGroup('app') } catch {}
  toast.add({ group: 'app', severity, summary, detail, life })
}

// ------- Estado -------
const usuarios = ref([])
const loading = ref(false)
const saving = ref(false)
const editingId = ref(null)

const model = reactive({
  descripcion: '',
  tipo: 'Administrador',
  correoElectronico: '',
  telefono: '',
  activo: true
})

const tipos = [
  { label: 'Administrador', value: 'Administrador' },
  { label: 'Cliente', value: 'Cliente' },
  { label: 'Agente',  value: 'Agente' }
]

// ------- Filtros -------
const filtroTipo = ref('Todos')
const filtroEstado = ref('Todos')
const opcionesTipo = [{ label: 'Todos', value: 'Todos' }, ...tipos]
const opcionesEstado = [
  { label: 'Todos', value: 'Todos' },
  { label: 'Activo', value: true },
  { label: 'Inactivo', value: false }
]
const filtrados = computed(() =>
  (usuarios.value ?? []).filter(u =>
    (filtroTipo.value === 'Todos' || u.tipo === filtroTipo.value) &&
    (filtroEstado.value === 'Todos' || u.activo === filtroEstado.value)
  )
)

// ------- Validación (solo al Guardar) -------
const emailRegex = /^[^\s@]+@[^\s@]+\.(com|[A-Za-z]{2,})$/
const telefonoRegex = /^\d{7,15}$/
const errors = computed(() => {
  const e = {}
  if (!model.descripcion?.trim()) e.descripcion = 'Requerido.'
  if (!tipos.some(t => t.value === model.tipo)) e.tipo = 'Tipo inválido.'
  if (!emailRegex.test(model.correoElectronico)) e.correo = 'Correo inválido.'
  if (!telefonoRegex.test(model.telefono)) e.telefono = 'Solo dígitos (7–15).'
  return e
})
const submitted = ref(false)
const showErr = f => submitted.value && errors.value[f]

// ------- CRUD -------
function resetForm() {
  editingId.value = null
  submitted.value = false
  Object.assign(model, {
    descripcion: '', tipo: 'Administrador', correoElectronico: '', telefono: '', activo: true
  })
}

async function load() {
  loading.value = true
  try {
    const { data } = await api.get('/usuarios')
    usuarios.value = Array.isArray(data) ? data : []
  } finally {
    loading.value = false
  }
}

async function submit() {
  submitted.value = true
  if (Object.keys(errors.value).length) {
    notify('warn', 'Validación', 'Revisá los campos')
    return
  }
  saving.value = true
  try {
    if (editingId.value === null) {
      const { data } = await api.post('/usuarios', { ...model })
      usuarios.value.push(data)
      resetForm()
      notify('success', 'Guardado', 'Usuario creado', 1800)
    } else {
      const { data } = await api.put(`/usuarios/${editingId.value}`, { ...model })
      const i = usuarios.value.findIndex(u => u.id === editingId.value)
      if (i >= 0) usuarios.value[i] = data
      resetForm()
      notify('success', 'Actualizado', '', 1800)
    }
  } catch (err) {
    if (axios.isAxiosError(err)) {
      if (err.code === 'ERR_NETWORK') { notify('error','API no disponible'); return }
      if (err.response?.status === 409) { notify('warn','Correo duplicado','Ya existe ese email'); return }
      if (err.response?.status === 400) { notify('warn','Validación','Revisá los campos'); return }
    }
    notify('error','Error al guardar')
  } finally {
    saving.value = false
  }
}

function edit(u) {
  editingId.value = u.id
  submitted.value = false
  Object.assign(model, {
    descripcion: u.descripcion,
    tipo: u.tipo,
    correoElectronico: u.correoElectronico,
    telefono: u.telefono,
    activo: u.activo
  })
}

function confirmDelete(id) {
  confirm.require({
    message: '¿Eliminar usuario?',
    header: 'Confirmar',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Eliminar',
    rejectLabel: 'Cancelar',
    acceptClass: 'p-button-danger',
    accept: () => removeUser(id)
  })
}

async function removeUser(id) {
  try {
    await api.delete(`/usuarios/${id}`)
    usuarios.value = usuarios.value.filter(u => u.id !== id)
    if (editingId.value === id) resetForm()
    notify('success','Eliminado','',1800)
  } catch {
    notify('error','Error al eliminar')
  }
}

// solo dígitos
function onlyDigits(e) {
  const v = (e.target.value || '').replace(/\D/g, '').slice(0, 15)
  model.telefono = v
}

onMounted(load)
</script>

<template>
  <div class="page p-fluid">
    <Toast group="app" position="top-right" />
    <ConfirmDialog />

    <div class="topbar">
      <div class="spacer"></div>
      <Button :label="dark ? 'Modo claro' : 'Modo oscuro'"
              :icon="dark ? 'pi pi-sun' : 'pi pi-moon'"
              size="small" text @click="dark = !dark" />
    </div>

    <Card class="card">
      <template #title>Usuarios</template>
      <template #content>
        <div class="grid">
          <div class="field">
            <label class="lbl">Descripción</label>
            <InputText v-model="model.descripcion"
                       placeholder="Nombre / Descripción"
                       :class="{ 'p-invalid': showErr('descripcion') }" />
            <InlineMessage v-if="showErr('descripcion')" severity="error">{{ errors.descripcion }}</InlineMessage>
          </div>

          <div class="field">
            <label class="lbl">Tipo</label>
            <Dropdown v-model="model.tipo"
                      :options="tipos" optionLabel="label" optionValue="value"
                      :class="{ 'p-invalid': showErr('tipo') }" />
            <InlineMessage v-if="showErr('tipo')" severity="error">{{ errors.tipo }}</InlineMessage>
          </div>

          <div class="field">
            <label class="lbl">Correo Electrónico</label>
            <InputText v-model="model.correoElectronico"
                       type="email" placeholder="usuario@dominio.com"
                       :class="{ 'p-invalid': showErr('correo') }" />
            <InlineMessage v-if="showErr('correo')" severity="error">{{ errors.correo }}</InlineMessage>
          </div>

          <div class="field">
            <label class="lbl">Teléfono</label>
            <InputText :value="model.telefono" @input="onlyDigits"
                       inputmode="numeric" placeholder="Sólo números"
                       :class="{ 'p-invalid': showErr('telefono') }" />
            <InlineMessage v-if="showErr('telefono')" severity="error">{{ errors.telefono }}</InlineMessage>
          </div>

          <div class="field switch">
           <Checkbox v-model="model.activo" :binary="true" />
           <label class="ml-2">Activo</label>
          </div>

          <div class="actions">
            <Button label="Guardar" icon="pi pi-save" :disabled="saving" @click="submit" />
            <Button v-if="editingId !== null" label="Cancelar" icon="pi pi-times"
                    severity="secondary" text @click="resetForm" />
          </div>
        </div>
      </template>
    </Card>

    <Card class="card">
      <template #title>Listado</template>
      <template #content>
        <!-- Filtros -->
        <div class="filters">
          <Dropdown v-model="filtroTipo" :options="opcionesTipo" optionLabel="label" optionValue="value" placeholder="Tipo" />
          <Dropdown v-model="filtroEstado" :options="opcionesEstado" optionLabel="label" optionValue="value" placeholder="Estado" />
          <Button label="Limpiar" size="small" text @click="filtroTipo='Todos'; filtroEstado='Todos'" />
        </div>

        <DataTable
          :value="filtrados"
          :loading="loading"
          dataKey="id"
          size="small"
          stripedRows
          paginator
          :rows="10"
        >
          <Column field="id" header="Id" style="width: 90px" />
          <Column field="descripcion" header="Descripción" />
          <Column field="tipo" header="Tipo" />
          <Column field="correoElectronico" header="Correo" />
          <Column field="telefono" header="Teléfono" />
          <Column header="Activo" style="width:120px">
            <template #body="{ data }">
              <Tag :value="data.activo ? 'Sí' : 'No'" :severity="data.activo ? 'success' : 'danger'" />
            </template>
          </Column>
          <Column header="Acciones" style="width:140px">
            <template #body="{ data }">
              <div class="row-actions">
                <Button icon="pi pi-pencil" rounded text @click="edit(data)" />
                <Button icon="pi pi-trash" rounded text severity="danger" @click="confirmDelete(data.id)" />
              </div>
            </template>
          </Column>
          <template #empty>Sin datos</template>
        </DataTable>
      </template>
    </Card>
  </div>
</template>

<style scoped>
.page   { max-width: 1100px; margin: 16px auto; padding: 0 12px; }
.topbar { display: flex; align-items: center; margin: 6px 0 12px; }
.topbar .spacer { flex: 1; }
.card   { margin-bottom: 20px; }
.grid   { display: grid; grid-template-columns: repeat(12, 1fr); gap: 14px; }
.field  { grid-column: span 6; display: flex; flex-direction: column; gap: 6px; }
.switch { grid-column: span 3; align-items: center; flex-direction: row; gap: 12px; }
.actions{ grid-column: span 12; display: flex; gap: 10px; margin-top: 6px; }
.lbl    { font-weight: 600; }
.row-actions { display: flex; gap: 6px; }
.filters { display:flex; gap:8px; align-items:center; margin-bottom:10px; }
</style>
