import { useAuth } from '../auth/AuthContext'
import { useCallback } from 'react'

export function useApi() {
  const { auth } = useAuth()

  const request = useCallback(async function request<T>(path: string, init?: RequestInit): Promise<T> {
    const res = await fetch(path, {
      ...init,
      headers: {
        'Content-Type': 'application/json',
        ...(auth.accessToken ? { Authorization: `Bearer ${auth.accessToken}` } : {}),
        ...(init?.headers || {}),
      },
    })
    const json = await res.json()
    if (!res.ok || json?.success === false) {
      const message = json?.message || 'Request failed'
      throw new Error(message)
    }
    return (json?.data ?? json) as T
  }, [auth.accessToken])

  return { request }
}

export type JobStatus = 'Applied' | 'InterviewScheduled' | 'InterviewCompleted' | 'OfferReceived' | 'Rejected' | 'Withdrawn'

export type UserJob = {
  jobId: string
  company: string
  position: string
  status: JobStatus
  applicationDate: string
  lastUpdated: string
  notes?: string
  salary?: number
  contact?: string
}

export type JobsList = {
  jobs: UserJob[]
  page: number
  pageSize: number
  total: number
}


