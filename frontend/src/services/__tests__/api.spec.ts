import { describe, it, expect, vi, beforeEach } from 'vitest'

// api.ts calls axios.create() at module-load time, then registers a
// request interceptor and a response interceptor on the returned instance.
// To test those interceptor functions directly (without a real network
// call), we fake axios.create() to hand back an instance we control, and
// capture whatever functions api.ts registers on it.
//
// vi.mock(...) factories are hoisted above imports/consts, so anything the
// factory closes over has to be created with vi.hoisted() - otherwise we'd
// hit a "Cannot access before initialization" error.
const { mockInstance } = vi.hoisted(() => {
  const instance = vi.fn() as any
  instance.interceptors = {
    request: { use: vi.fn() },
    response: { use: vi.fn() },
  }
  return { mockInstance: instance }
})

vi.mock('axios', () => ({
  default: {
    create: vi.fn(() => mockInstance),
  },
}))

vi.mock('@/services/authService', () => ({
  getToken: vi.fn(),
  setToken: vi.fn(),
  refreshAccessToken: vi.fn(),
  logout: vi.fn(),
}))

describe('api interceptors', () => {
  let requestInterceptor: (config: any) => any
  let responseErrorInterceptor: (error: any) => any

  beforeEach(async () => {
    vi.clearAllMocks()
    mockInstance.interceptors.request.use.mockClear()
    mockInstance.interceptors.response.use.mockClear()

    // Re-import api.ts fresh so it re-registers its interceptors on our
    // mock instance every test.
    vi.resetModules()
    await import('@/services/api')

    requestInterceptor = mockInstance.interceptors.request.use.mock.calls[0][0]
    responseErrorInterceptor = mockInstance.interceptors.response.use.mock.calls[0][1]
  })

  it('creates the axios instance with a relative baseURL and credentials enabled', async () => {
    const axios = (await import('axios')).default
    expect(axios.create).toHaveBeenCalledWith({ baseURL: '/api', withCredentials: true })
  })

  it('attaches an Authorization header when a token exists', async () => {
    const { getToken } = await import('@/services/authService')
    vi.mocked(getToken).mockReturnValue('my-token')

    const config = requestInterceptor({ headers: {} })

    expect(config.headers['Authorization']).toBe('Bearer my-token')
  })

  it('does not attach an Authorization header when there is no token', async () => {
    const { getToken } = await import('@/services/authService')
    vi.mocked(getToken).mockReturnValue(null)

    const config = requestInterceptor({ headers: {} })

    expect(config.headers['Authorization']).toBeUndefined()
  })

  it('on a 401, refreshes the token via the httpOnly cookie and retries the original request', async () => {
    const { refreshAccessToken, setToken } = await import('@/services/authService')
    vi.mocked(refreshAccessToken).mockResolvedValue('fresh-token')
    mockInstance.mockResolvedValue({ data: 'retried-response' })

    const originalRequest: any = { headers: {}, url: '/workouts' }
    const error = { response: { status: 401 }, config: originalRequest }

    const result = await responseErrorInterceptor(error)

    expect(refreshAccessToken).toHaveBeenCalled()
    expect(setToken).toHaveBeenCalledWith('fresh-token')
    expect(originalRequest.headers['Authorization']).toBe('Bearer fresh-token')
    expect(mockInstance).toHaveBeenCalledWith(originalRequest)
    expect(result).toEqual({ data: 'retried-response' })
  })

  it('logs out and rejects when there is no valid refresh session', async () => {
    const { refreshAccessToken, logout } = await import('@/services/authService')
    vi.mocked(refreshAccessToken).mockRejectedValue(new Error('no valid session'))

    const originalRequest = { headers: {}, url: '/workouts' }
    const error = { response: { status: 401 }, config: originalRequest }

    await expect(responseErrorInterceptor(error)).rejects.toThrow('no valid session')
    expect(logout).toHaveBeenCalled()
  })

  it('passes non-401 errors straight through', async () => {
    const originalRequest = { headers: {}, url: '/workouts' }
    const error = { response: { status: 500 }, config: originalRequest }

    await expect(responseErrorInterceptor(error)).rejects.toBe(error)
  })
})
