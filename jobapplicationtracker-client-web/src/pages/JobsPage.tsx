import { useEffect, useMemo, useState } from 'react'
import { useApi } from '../api/client'
import type { JobsList, UserJob } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import ThemeToggle from '../components/ThemeToggle'

export default function JobsPage() {
  const { request } = useApi()
  const { logout, auth } = useAuth()
  const [data, setData] = useState<JobsList | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [page, setPage] = useState(1)
  const [status, setStatus] = useState<string>('')
  const [statuses, setStatuses] = useState<string[]>([])
  const [showForm, setShowForm] = useState(false)
  const [editing, setEditing] = useState<UserJob | null>(null)
  const [notesJob, setNotesJob] = useState<UserJob | null>(null)
  const [currency, setCurrency] = useState<string>(() => localStorage.getItem('jat.currency') || 'USD')

  useEffect(() => {
    localStorage.setItem('jat.currency', currency)
  }, [currency])

  function formatCurrency(value?: number | null) {
    if (value == null) return '-'
    try {
      return new Intl.NumberFormat('en-US', { style: 'currency', currency: (currency as any), maximumFractionDigits: 0 }).format(value)
    } catch {
      const symbol = currency === 'EUR' ? '€' : currency === 'GBP' ? '£' : currency === 'BGN' ? 'лв' : '$'
      return `${symbol}${value}`
    }
  }

  type JobForm = {
    company: string
    position: string
    status: string
    applicationDate: string
    notes?: string
    salary?: string
    contact?: string
  }
  const [form, setForm] = useState<JobForm>({ company: '', position: '', status: 'Applied', applicationDate: new Date().toISOString().slice(0, 10), notes: '', salary: '', contact: '' })

  const query = useMemo(() => {
    const p = new URLSearchParams()
    p.set('page', String(page))
    p.set('pageSize', '10')
    if (status) p.set('status', status)
    return p.toString()
  }, [page, status])

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    setError(null)
    request<JobsList>(`/api/Jobs/GetAllUserJobs?${query}`)
      .then(setData)
      .catch(e => setError(e.message))
      .finally(() => !cancelled && setLoading(false))
    return () => { cancelled = true }
  }, [query, request])

  useEffect(() => {
    request<string[]>(`/api/Jobs/Statuses`).then(setStatuses).catch(() => {})
  }, [request])

  function onLogout() {
    logout()
  }

  function openCreate() {
    setEditing(null)
    setForm({ company: '', position: '', status: 'Applied', applicationDate: new Date().toISOString().slice(0, 10), notes: '', salary: '', contact: '' })
    setShowForm(true)
  }

  function openEdit(job: UserJob) {
    setEditing(job)
    setForm({
      company: job.company,
      position: job.position,
      status: job.status,
      applicationDate: job.applicationDate.slice(0, 10),
      notes: job.notes ?? '',
      salary: job.salary != null ? String(job.salary) : '',
      contact: job.contact ?? '',
    })
    setShowForm(true)
  }

  async function submitForm(e: React.FormEvent) {
    e.preventDefault()
    try {
      const bodyBase = {
        company: form.company,
        position: form.position,
        status: form.status,
        applicationDate: new Date(form.applicationDate).toISOString(),
        notes: form.notes || null,
        salary: form.salary ? Number(form.salary) : null,
        contact: form.contact || null,
      }
      if (editing) {
        await request<UserJob>('/api/Jobs/UpdateJob', {
          method: 'PUT',
          body: JSON.stringify({ ...bodyBase, jobId: editing.jobId }),
        })
      } else {
        await request<UserJob>('/api/Jobs/CreateJob', {
          method: 'POST',
          body: JSON.stringify(bodyBase),
        })
      }
      setShowForm(false)
      // refresh list
      const refresh = await request<JobsList>(`/api/Jobs/GetAllUserJobs?${query}`)
      setData(refresh)
    } catch (e: any) {
      setError(e.message || 'Operation failed')
    }
  }

  async function onDelete(job: UserJob) {
    if (!confirm(`Delete job at ${job.company}?`)) return
    try {
      await request<null>(`/api/Jobs/DeleteJob?id=${encodeURIComponent(job.jobId)}`, { method: 'DELETE' })
      const refresh = await request<JobsList>(`/api/Jobs/GetAllUserJobs?${query}`)
      setData(refresh)
    } catch (e: any) {
      setError(e.message || 'Delete failed')
    }
  }

  return (
    <div>
      <div className="toolbar" style={{ flexWrap: 'wrap', gap: 8 }}>
        <h2 style={{ margin: 0 }}>Job Application Tracker</h2>
        <div style={{ display: 'flex', gap: 12, alignItems: 'center' }}>
          <span className="muted">{auth.email}</span>
          <button className="btn" onClick={onLogout}>Logout</button>
        </div>
        <div style={{ marginLeft: 'auto' }}>
          <ThemeToggle />
        </div>
      </div>

      <div className="container">
        <section className="row" style={{ marginBottom: 16 }}>
          <select className="input" value={status} onChange={e => setStatus(e.target.value)}>
            <option value="">All statuses</option>
            {statuses.map(s => (
              <option key={s} value={s}>{s.replace(/([A-Z])/g, ' $1').trim()}</option>
            ))}
          </select>
          <select className="input" value={currency} onChange={e => setCurrency(e.target.value)} title="Currency">
            <option value="USD">USD</option>
            <option value="EUR">EUR</option>
            <option value="GBP">GBP</option>
            <option value="BGN">BGN</option>
          </select>
          <button className="btn primary" onClick={openCreate}>Add Job</button>
        </section>

        {showForm && (
          <form onSubmit={submitForm} style={{ marginBottom: 16 }}>
            <h3 style={{ marginTop: 0 }}>{editing ? 'Edit Job' : 'Add Job'}</h3>
            <div className="grid2">
              <input className="input" placeholder="Company" value={form.company} onChange={e => setForm(f => ({ ...f, company: e.target.value }))} required />
              <input className="input" placeholder="Position" value={form.position} onChange={e => setForm(f => ({ ...f, position: e.target.value }))} required />
              <select className="input" value={form.status} onChange={e => setForm(f => ({ ...f, status: e.target.value }))}>
                {statuses.map(s => (
                  <option key={s} value={s}>{s.replace(/([A-Z])/g, ' $1').trim()}</option>
                ))}
              </select>
              <input className="input" type="date" value={form.applicationDate} onChange={e => setForm(f => ({ ...f, applicationDate: e.target.value }))} />
              <input className="input" placeholder="Salary" type="number" min="0" step="0.01" value={form.salary ?? ''} onChange={e => setForm(f => ({ ...f, salary: e.target.value }))} />
              <input className="input" placeholder="Contact" value={form.contact ?? ''} onChange={e => setForm(f => ({ ...f, contact: e.target.value }))} />
              <textarea className="input" placeholder="Notes" style={{ gridColumn: '1 / -1' }} value={form.notes ?? ''} onChange={e => setForm(f => ({ ...f, notes: e.target.value }))} />
            </div>
            <div style={{ display: 'flex', gap: 8, marginTop: 12 }}>
              <button className="btn primary" type="submit">{editing ? 'Save Changes' : 'Create Job'}</button>
              <button className="btn" type="button" onClick={() => setShowForm(false)}>Cancel</button>
            </div>
          </form>
        )}

        {loading && <div>Loading...</div>}
        {error && <div style={{ color: 'crimson' }}>{error}</div>}

        {!loading && !error && data && (
          <>
            <div className="tableWrap">
            <table className="table">
              <thead>
                <tr>
                  <th>Company</th>
                  <th>Position</th>
                  <th>Status</th>
                  <th>Applied</th>
                  <th>Salary</th>
                  <th>Contact</th>
                </tr>
              </thead>
              <tbody>
                {data.jobs.map((j: UserJob) => (
                  <tr key={j.jobId} onDoubleClick={() => openEdit(j)}>
                    <td><span className="cellLabel">Company</span>{j.company}</td>
                    <td><span className="cellLabel">Position</span>{j.position}</td>
                    <td>
                      <span className="cellLabel">Status</span>
                      <span className={`badge ${j.status === 'Applied' ? 'blue' : j.status === 'InterviewScheduled' ? 'indigo' : j.status === 'InterviewCompleted' ? 'green' : j.status === 'OfferReceived' ? 'amber' : j.status === 'Rejected' ? 'red' : 'gray'}`}>
                        {String(j.status).replace(/([A-Z])/g, ' $1').trim()}
                      </span>
                    </td>
                    <td><span className="cellLabel">Applied</span>{new Date(j.applicationDate).toLocaleDateString()}</td>
                    <td><span className="cellLabel">Salary</span>{formatCurrency(j.salary as number)}</td>
                    <td>
                      <span className="cellLabel">Actions</span>
                      <div className="actions">
                        <button className="btn" onClick={() => openEdit(j)}>Edit</button>
                        <button className="btn danger" onClick={() => onDelete(j)}>Delete</button>
                        <button className="btn" onClick={() => setNotesJob(j)}>View Notes</button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            </div>

            <div style={{ display: 'flex', justifyContent: 'space-between', gap: 8, marginTop: 12 }}>
              <div className="muted">Total: {data.total}</div>
              <div style={{ display: 'flex', gap: 8 }}>
                <button className="btn" disabled={page <= 1} onClick={() => setPage(p => Math.max(1, p - 1))}>Prev</button>
                <span className="muted" style={{ alignSelf: 'center' }}>Page {data.page} / {Math.max(1, Math.ceil(data.total / data.pageSize))}</span>
                <button className="btn" disabled={data.page * data.pageSize >= data.total} onClick={() => setPage(p => p + 1)}>Next</button>
              </div>
            </div>
          </>
        )}
      </div>

      {notesJob && (
        <div className="modalBackdrop" onClick={() => setNotesJob(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="toolbar">
              <h3 style={{ margin: 0 }}>Notes - {notesJob.company}</h3>
              <button className="btn" onClick={() => setNotesJob(null)}>Close</button>
            </div>
            <div className="grid1">
              <div className="muted">Position: {notesJob.position}</div>
              <div className="muted">Status: {String(notesJob.status).replace(/([A-Z])/g, ' $1').trim()}</div>
              <div className="muted">Applied: {new Date(notesJob.applicationDate).toLocaleString()}</div>
              <div style={{ whiteSpace: 'pre-wrap', background: '#0b1220', border: '1px solid #1f2937', borderRadius: 8, padding: 12 }}>
                {notesJob.notes ? notesJob.notes : 'No notes added.'}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}


