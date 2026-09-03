import { useState, type FormEvent } from "react";
import { useNavigate, Link } from "react-router-dom";
import { registerUser } from "../api/auth";
import { useAuthStore } from "../store/authStore";

export default function RegisterPage() {
  const [tenantName, setTenantName] = useState("");
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const setTokens = useAuthStore((s) => s.setTokens);
  const navigate = useNavigate();

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    try {
      const data = await registerUser({ tenantName, fullName, email, password });
      setTokens(data.accessToken, data.refreshToken);
      navigate("/projects");
    } catch (err: any) {
      const errors = err.response?.data?.errors;
      setError(Array.isArray(errors) ? errors.join(" ") : err.response?.data?.error ?? "Registration failed.");
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50">
      <form onSubmit={handleSubmit} className="w-full max-w-sm bg-white p-8 rounded-lg shadow">
        <h1 className="text-2xl font-semibold mb-6">Create your workspace</h1>
        {error && <p className="text-red-600 text-sm mb-4">{error}</p>}
        <input type="text" placeholder="Organization name" value={tenantName} onChange={(e) => setTenantName(e.target.value)}
          className="w-full border border-slate-300 rounded px-3 py-2 mb-3" required />
        <input type="text" placeholder="Your full name" value={fullName} onChange={(e) => setFullName(e.target.value)}
          className="w-full border border-slate-300 rounded px-3 py-2 mb-3" required />
        <input type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)}
          className="w-full border border-slate-300 rounded px-3 py-2 mb-3" required />
        <input type="password" placeholder="Password (min 8 characters)" value={password} onChange={(e) => setPassword(e.target.value)}
          className="w-full border border-slate-300 rounded px-3 py-2 mb-4" required />
        <button type="submit" className="w-full bg-slate-900 text-white rounded py-2 font-medium hover:bg-slate-800">
          Create account
        </button>
        <p className="text-sm text-slate-600 mt-4 text-center">
          Already have an account? <Link to="/login" className="text-slate-900 underline">Log in</Link>
        </p>
      </form>
    </div>
  );
}