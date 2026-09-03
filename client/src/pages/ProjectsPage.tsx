import { useState, type FormEvent } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getProjects, createProject } from "../api/projects";
import { useAuthStore } from "../store/authStore";
import { useNavigate, Link } from "react-router-dom";

export default function ProjectsPage() {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const logout = useAuthStore((s) => s.logout);
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const {
    data: projects,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["projects"],
    queryFn: getProjects,
  });

  const createMutation = useMutation({
    mutationFn: createProject,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      setName("");
      setDescription("");
    },
  });

  function handleCreate(e: FormEvent) {
    e.preventDefault();
    if (!name.trim()) return;
    createMutation.mutate({ name, description: description || undefined });
  }

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <div className="min-h-screen bg-slate-50 p-8">
      <div className="max-w-3xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-semibold">Your Projects</h1>
          <button
            onClick={handleLogout}
            className="text-sm text-slate-600 underline"
          >
            Log out
          </button>
        </div>

        <form
          onSubmit={handleCreate}
          className="bg-white p-4 rounded-lg shadow mb-6 flex gap-2"
        >
          <input
            type="text"
            placeholder="New project name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="flex-1 border border-slate-300 rounded px-3 py-2"
          />
          <input
            type="text"
            placeholder="Description (optional)"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="flex-1 border border-slate-300 rounded px-3 py-2"
          />
          <button
            type="submit"
            disabled={createMutation.isPending}
            className="bg-slate-900 text-white rounded px-4 py-2 font-medium hover:bg-slate-800 disabled:opacity-50"
          >
            {createMutation.isPending ? "Creating..." : "Create"}
          </button>
        </form>

        {isLoading && <p className="text-slate-600">Loading projects...</p>}
        {isError && <p className="text-red-600">Failed to load projects.</p>}

        <div className="grid gap-3">
          {projects?.map((project) => (
            <Link
              key={project.id}
              to={`/projects/${project.id}`}
              className="block bg-white p-4 rounded-lg shadow hover:shadow-md transition-shadow"
            >
              <h2 className="font-medium">{project.name}</h2>
              {project.description && (
                <p className="text-slate-600 text-sm mt-1">
                  {project.description}
                </p>
              )}
            </Link>
          ))}
          {projects?.length === 0 && (
            <p className="text-slate-500 text-sm">
              No projects yet — create your first one above.
            </p>
          )}
        </div>
      </div>
    </div>
  );
}