import { useState, type FormEvent } from "react";
import { useNavigate, Link } from "react-router-dom";
import { loginUser } from "../api/auth";
import { useAuthStore } from "../store/authStore";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const setTokens = useAuthStore((s) => s.setTokens);
  const navigate = useNavigate();

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    try {
      const data = await loginUser({ email, password });
      setTokens(data.accessToken, data.refreshToken);
      navigate("/projects");
    } catch (err: any) {
      setError(err.response?.data?.error ?? "Login failed. Check your credentials.");
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50">
      <form onSubmit={handleSubmit} className="w-full max-w-sm bg-white p-8 rounded-lg shadow">
        <h1 className="text-2xl font-semibold mb-6">Log in to TaskForge</h1>
        {error && <p className="text-red-600 text-sm mb-4">{error}</p>}
        <input type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)}
          className="w-full border border-slate-300 rounded px-3 py-2 mb-3" required />
        <input type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)}
          className="w-full border border-slate-300 rounded px-3 py-2 mb-4" required />
        <button type="submit" className="w-full bg-slate-900 text-white rounded py-2 font-medium hover:bg-slate-800">
          Log in
        </button>
        <p className="text-sm text-slate-600 mt-4 text-center">
          No account? <Link to="/register" className="text-slate-900 underline">Register</Link>
        </p>
      </form>
    </div>
  );
}