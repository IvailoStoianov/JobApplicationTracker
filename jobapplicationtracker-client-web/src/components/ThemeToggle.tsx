import { useEffect, useState } from 'react'

export default function ThemeToggle() {
  const [theme, setTheme] = useState<string>(() => localStorage.getItem('jat.theme') || 'dark')

  useEffect(() => {
    document.body.setAttribute('data-theme', theme === 'light' ? 'light' : 'dark')
    localStorage.setItem('jat.theme', theme)
  }, [theme])

  const checked = theme === 'light'

  return (
    <label className="themeSwitch">
      <input type="checkbox" checked={checked} onChange={() => setTheme(t => (t === 'light' ? 'dark' : 'light'))} />
      <div className="track">
        <div className="thumb" />
      </div>
      <span className="muted">{checked ? 'Light' : 'Dark'}</span>
    </label>
  )
}


