import { useEffect, useMemo, useState } from 'react'
import { useApi } from '../api/client'
import type { JobsList, UserJob } from '../api/client'
import { useAuth } from '../auth/AuthContext'

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
    <div style={{ maxWidth: 960, margin: '4vh auto', padding: 24 }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <h2 style={{ margin: 0 }}>My Applications</h2>
        <div style={{ display: 'flex', gap: 12, alignItems: 'center' }}>
          <span style={{ fontSize: 14, opacity: 0.8 }}>{auth.email}</span>
          <button onClick={onLogout}>Logout</button>
        </div>
      </header>

      <section style={{ display: 'flex', gap: 12, marginBottom: 16 }}>
        <select value={status} onChange={e => setStatus(e.target.value)}>
          <option value="">All statuses</option>
          {statuses.map(s => (
            <option key={s} value={s}>{s.replace(/([A-Z])/g, ' $1').trim()}</option>
          ))}
        </select>
        <button onClick={openCreate}>Add Job</button>
      </section>

      {showForm && (
        <form onSubmit={submitForm} style={{ border: '1px solid #e5e5e5', padding: 16, borderRadius: 8, marginBottom: 16 }}>
          <h3 style={{ marginTop: 0 }}>{editing ? 'Edit Job' : 'Add Job'}</h3>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
            <input placeholder="Company" value={form.company} onChange={e => setForm(f => ({ ...f, company: e.target.value }))} required />
            <input placeholder="Position" value={form.position} onChange={e => setForm(f => ({ ...f, position: e.target.value }))} required />
            <select value={form.status} onChange={e => setForm(f => ({ ...f, status: e.target.value }))}>
              {statuses.map(s => (
                <option key={s} value={s}>{s.replace(/([A-Z])/g, ' $1').trim()}</option>
              ))}
            </select>
            <input type="date" value={form.applicationDate} onChange={e => setForm(f => ({ ...f, applicationDate: e.target.value }))} />
            <input placeholder="Salary" type="number" min="0" step="0.01" value={form.salary ?? ''} onChange={e => setForm(f => ({ ...f, salary: e.target.value }))} />
            <input placeholder="Contact" value={form.contact ?? ''} onChange={e => setForm(f => ({ ...f, contact: e.target.value }))} />
            <textarea placeholder="Notes" style={{ gridColumn: '1 / -1' }} value={form.notes ?? ''} onChange={e => setForm(f => ({ ...f, notes: e.target.value }))} />
          </div>
          <div style={{ display: 'flex', gap: 8, marginTop: 12 }}>
            <button type="submit">{editing ? 'Save Changes' : 'Create Job'}</button>
            <button type="button" onClick={() => setShowForm(false)}>Cancel</button>
          </div>
        </form>
      )}

      {loading && <div>Loading...</div>}
      {error && <div style={{ color: 'crimson' }}>{error}</div>}

      {!loading && !error && data && (
        <>
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <thead>
              <tr>
                <th style={{ textAlign: 'left', padding: 8, borderBottom: '1px solid #ddd' }}>Company</th>
                <th style={{ textAlign: 'left', padding: 8, borderBottom: '1px solid #ddd' }}>Position</th>
                <th style={{ textAlign: 'left', padding: 8, borderBottom: '1px solid #ddd' }}>Status</th>
                <th style={{ textAlign: 'left', padding: 8, borderBottom: '1px solid #ddd' }}>Applied</th>
                <th style={{ textAlign: 'left', padding: 8, borderBottom: '1px solid #ddd' }}>Salary</th>
                <th style={{ textAlign: 'left', padding: 8, borderBottom: '1px solid #ddd' }}>Contact</th>
              </tr>
            </thead>
            <tbody>
              {data.jobs.map((j: UserJob) => (
                <tr key={j.jobId} onDoubleClick={() => openEdit(j)}>
                  <td style={{ padding: 8, borderBottom: '1px solid #f0f0f0' }}>{j.company}</td>
                  <td style={{ padding: 8, borderBottom: '1px solid #f0f0f0' }}>{j.position}</td>
                  <td style={{ padding: 8, borderBottom: '1px solid #f0f0f0' }}>{j.status}</td>
                  <td style={{ padding: 8, borderBottom: '1px solid #f0f0f0' }}>{new Date(j.applicationDate).toLocaleDateString()}</td>
                  <td style={{ padding: 8, borderBottom: '1px solid #f0f0f0' }}>{j.salary ?? '-'}</td>
                  <td style={{ padding: 8, borderBottom: '1px solid #f0f0f0' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: 8 }}>
                      <span>{j.contact ?? '-'}</span>
                      <span>
                        <button onClick={() => openEdit(j)}>Edit</button>
                        <button onClick={() => onDelete(j)} style={{ marginLeft: 6 }}>Delete</button>
                      </span>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, marginTop: 12 }}>
            <button disabled={page <= 1} onClick={() => setPage(p => Math.max(1, p - 1))}>Prev</button>
            <span style={{ alignSelf: 'center' }}>Page {data.page} / {Math.max(1, Math.ceil(data.total / data.pageSize))}</span>
            <button disabled={data.page * data.pageSize >= data.total} onClick={() => setPage(p => p + 1)}>Next</button>
          </div>
        </>
      )}
    </div>
  )
}


