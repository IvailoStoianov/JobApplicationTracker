import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

export default function LoginPage() {
  const { login } = useAuth()
  const nav = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault()
    setLoading(true)
    setError(null)
    const ok = await login(email, password)
    setLoading(false)
    if (ok) nav('/')
    else setError('Invalid email or password')
  }

  return (
    <div style={{ maxWidth: 420, margin: '10vh auto', padding: 24 }}>
      <h2 style={{ marginBottom: 16 }}>Job Tracker - Login</h2>
      <form onSubmit={onSubmit}>
        <div style={{ display: 'grid', gap: 12 }}>
          <input placeholder="Email" type="email" value={email} onChange={e => setEmail(e.target.value)} required />
          <input placeholder="Password" type="password" value={password} onChange={e => setPassword(e.target.value)} required />
          <button type="submit" disabled={loading}>{loading ? 'Signing in...' : 'Sign in'}</button>
          {error && <div style={{ color: 'crimson' }}>{error}</div>}
          <button type="button" onClick={() => nav('/register')} style={{ background: 'transparent', color: '#646cff' }}>Create an account</button>
        </div>
      </form>
    </div>
  )
}


