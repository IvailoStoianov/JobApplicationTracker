import { createContext, useContext, useEffect, useMemo, useState } from 'react'

type AuthState = {
  accessToken: string | null
  email: string | null
}

type AuthContextValue = {
  isAuthenticated: boolean
  auth: AuthState
  login: (email: string, password: string) => Promise<boolean>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [auth, setAuth] = useState<AuthState>(() => {
    const token = localStorage.getItem('jat.token')
    const email = localStorage.getItem('jat.email')
    return { accessToken: token, email }
  })

  useEffect(() => {
    if (auth.accessToken) localStorage.setItem('jat.token', auth.accessToken)
    else localStorage.removeItem('jat.token')
    if (auth.email) localStorage.setItem('jat.email', auth.email)
    else localStorage.removeItem('jat.email')
  }, [auth])

  const value = useMemo<AuthContextValue>(() => ({
    isAuthenticated: !!auth.accessToken,
    auth,
    login: async (email: string, password: string) => {
      try {
        const res = await fetch('/api/Authentication/Login', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ email, password }),
        })
        const json = await res.json()
        if (!res.ok || !json?.success) return false
        const data = json.data as { accessToken: string; email: string }
        setAuth({ accessToken: data.accessToken, email: data.email })
        return true
      } catch {
        return false
      }
    },
    logout: () => setAuth({ accessToken: null, email: null }),
  }), [auth])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}


