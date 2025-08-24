import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

export default function RegisterPage() {
  const nav = useNavigate()
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [showPwd, setShowPwd] = useState(false)
  const [showPwd2, setShowPwd2] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [passwordHint, setPasswordHint] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault()
    setLoading(true)
    setError(null)
    setPasswordHint(null)
    try {
      const res = await fetch('/api/Authentication/Register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, email, password, confirmPassword }),
      })
      const json = await res.json()
      if (!res.ok || !json?.success) {
        // Show backend message if provided
        const msg = json?.message || 'Registration failed'
        // If password fails complexity, provide client-side hint too
        setPasswordHint('Password must be 8-50 chars and include uppercase, lowercase, number, and symbol.')
        throw new Error(msg)
      }
      nav('/login')
    } catch (e: any) {
      setError(e.message || 'Registration failed')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={{ maxWidth: 420, margin: '10vh auto', padding: 24 }}>
      <h2 style={{ marginBottom: 16 }}>Create your account</h2>
      <form onSubmit={onSubmit} autoComplete="off">
        <div style={{ display: 'grid', gap: 12 }}>
          <input className="input" placeholder="Username" name="username" autoComplete="username" value={username} onChange={e => setUsername(e.target.value)} required />
          <input className="input" placeholder="Email" name="email" autoComplete="email" type="email" value={email} onChange={e => setEmail(e.target.value)} required />
          <div className="inputWrap">
            <input className="input" placeholder="Password" name="new-password" autoComplete="new-password" type={showPwd ? 'text' : 'password'} value={password} onChange={e => setPassword(e.target.value)} required />
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
          <div className="inputWrap">
            <input className="input" placeholder="Confirm password" name="confirm-password" autoComplete="new-password" type={showPwd2 ? 'text' : 'password'} value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} required />
            <button type="button" className="eyeBtn" onClick={() => setShowPwd2(s => !s)} aria-label="Toggle password visibility">
              {showPwd2 ? (
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
          <button type="submit" disabled={loading}>{loading ? 'Creating...' : 'Register'}</button>
          {error && <div style={{ color: 'crimson' }}>{error}</div>}
          {passwordHint && <div className="muted" style={{ fontSize: 12 }}>{passwordHint}</div>}
          <button type="button" onClick={() => nav('/login')} style={{ background: 'transparent', color: '#646cff' }}>Back to login</button>
        </div>
      </form>
    </div>
  )
}


