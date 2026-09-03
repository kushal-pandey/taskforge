import { useState, type FormEvent } from "react";
import { useParams, Link } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getProjectById } from "../api/projects";
import { getBoards, createBoard } from "../api/boards";

export default function ProjectDetailPage() {
  const { projectId } = useParams<{ projectId: string }>();
  const [name, setName] = useState("");
  const queryClient = useQueryClient();

  const { data: project } = useQuery({
    queryKey: ["project", projectId],
    queryFn: () => getProjectById(projectId!),
    enabled: !!projectId,
  });

  const { data: boards, isLoading } = useQuery({
    queryKey: ["boards", projectId],
    queryFn: () => getBoards(projectId!),
    enabled: !!projectId,
  });

  const createMutation = useMutation({
    mutationFn: (boardName: string) => createBoard(projectId!, { name: boardName }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["boards", projectId] });
      setName("");
    },
  });

  function handleCreate(e: FormEvent) {
    e.preventDefault();
    if (!name.trim()) return;
    createMutation.mutate(name);
  }

  return (
    <div className="min-h-screen bg-slate-50 p-8">
      <div className="max-w-3xl mx-auto">
        <Link to="/projects" className="text-sm text-slate-600 underline">&larr; Back to projects</Link>
        <h1 className="text-2xl font-semibold mt-2 mb-6">{project?.name ?? "Loading..."}</h1>

        <form onSubmit={handleCreate} className="bg-white p-4 rounded-lg shadow mb-6 flex gap-2">
          <input
            type="text"
            placeholder="New board name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="flex-1 border border-slate-300 rounded px-3 py-2"
          />
          <button
            type="submit"
            disabled={createMutation.isPending}
            className="bg-slate-900 text-white rounded px-4 py-2 font-medium hover:bg-slate-800 disabled:opacity-50"
          >
            {createMutation.isPending ? "Creating..." : "Create Board"}
          </button>
        </form>

        {isLoading && <p className="text-slate-600">Loading boards...</p>}

        <div className="grid gap-3">
          {boards?.map((board) => (
            <Link
              key={board.id}
              to={`/projects/${projectId}/boards/${board.id}`}
              className="block bg-white p-4 rounded-lg shadow hover:shadow-md transition-shadow"
            >
              <h2 className="font-medium">{board.name}</h2>
            </Link>
          ))}
          {boards?.length === 0 && (
            <p className="text-slate-500 text-sm">No boards yet — create your first one above.</p>
          )}
        </div>
      </div>
    </div>
  );
}