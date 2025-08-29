import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

export default function LoginPage() {
  const { login } = useAuth()
  const nav = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [showPwd, setShowPwd] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [passwordHint, setPasswordHint] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault()
    setLoading(true)
    setError(null)
    setPasswordHint(null)
    const res = await login(email, password)
    setLoading(false)
    if (res.ok) nav('/')
    else {
      setError(res.message || 'Invalid email or password')
      setPasswordHint('Tip: Passwords must include uppercase, lowercase, number, and symbol.')
    }
  }

  return (
    <div style={{ maxWidth: 420, margin: '10vh auto', padding: 24 }}>
      <h2 style={{ marginBottom: 16 }}>Job Tracker - Login</h2>
      <div className="muted" style={{ marginBottom: 12 }}>
        Heads up: this personal project runs on free-tier resources and may start cold; the first request can take a few seconds.
      </div>
      <form onSubmit={onSubmit} autoComplete="off">
        <div style={{ display: 'grid', gap: 12 }}>
          <input className="input" placeholder="Email" name="email" autoComplete="email" type="email" value={email} onChange={e => setEmail(e.target.value)} required />
          <div className="inputWrap">
            <input className="input" placeholder="Password" name="current-password" autoComplete="current-password" type={showPwd ? 'text' : 'password'} value={password} onChange={e => setPassword(e.target.value)} required />
            <button type="button" className="eyeBtn" onClick={() => setShowPwd(s => !s)} aria-label="Toggle password visibility">
              {showPwd ? (
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path d="M2 12s3.6-7 10-7 10 7 10 7-3.6 7-10 7S2 12 2 12Z" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round"/>
                  <circle cx="12" cy="12" r="3" stroke="currentColor" strokeWidth="1.6"/>
                </svg>
              ) : (
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path d="M2 12s3.6-7 10-7 10 7 10 7-3.6 7-10 7S2 12 2 12Z" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round"/>
                  <circle cx="12" cy="12" r="3" stroke="currentColor" strokeWidth="1.6"/>
                  <path d="M4 4L20 20" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round"/>
                </svg>
              )}
            </button>
          </div>
          <button type="submit" disabled={loading}>{loading ? 'Signing in...' : 'Sign in'}</button>
          {error && <div style={{ color: 'crimson' }}>{error}</div>}
          {passwordHint && <div className="muted" style={{ fontSize: 12 }}>{passwordHint}</div>}
          <button type="button" onClick={() => nav('/register')} style={{ background: 'transparent', color: '#646cff' }}>Create an account</button>
        </div>
      </form>
    </div>
  )
}


