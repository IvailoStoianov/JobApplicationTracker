import './App.css'
import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom'
import { AuthProvider, useAuth } from './auth/AuthContext'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import JobsPage from './pages/JobsPage.tsx'
import type { ReactElement } from 'react'
import { useEffect, useState } from 'react'

function ProtectedRoute({ children }: { children: ReactElement }) {
  const { isAuthenticated } = useAuth()
  if (!isAuthenticated) return <Navigate to="/login" replace />
  return children
}

function WelcomeNotice() {
  const [show, setShow] = useState(false)
  useEffect(() => {
    const dismissed = localStorage.getItem('jat.welcomeDismissed')
    if (!dismissed) setShow(true)
  }, [])
  function close() {
    localStorage.setItem('jat.welcomeDismissed', '1')
    setShow(false)
  }
  if (!show) return null
  return (
    <div className="modalBackdrop" onClick={close}>
      <div className="modal" onClick={e => e.stopPropagation()}>
        <div className="toolbar">
          <h3 style={{ margin: 0 }}>Welcome!</h3>
          <button className="btn" onClick={close}>Got it</button>
        </div>
        <div className="grid1" style={{ gap: 12 }}>
          <div className="muted">
            This app is a personal CV/showcase project hosted on a free tier. The backend may be idle between requests, so the very first action can take a few seconds while it wakes up.
          </div>
          <div className="muted">
            Thanks for your patience! Subsequent actions should be faster once the server is warm.
          </div>
        </div>
      </div>
    </div>
  )
}

export default function App() {
  return (
    <AuthProvider>
      <WelcomeNotice />
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <JobsPage />
              </ProtectedRoute>
            }
          />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}
