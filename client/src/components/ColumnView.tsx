import { useState, type FormEvent } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getTasks, createTask } from "../api/tasks";
import type { Column } from "../api/columns";

interface ColumnViewProps {
  column: Column;
}

export default function ColumnView({ column }: ColumnViewProps) {
  const [title, setTitle] = useState("");
  const queryClient = useQueryClient();

  const { data: tasks, isLoading } = useQuery({
    queryKey: ["tasks", column.id],
    queryFn: () => getTasks(column.id),
  });

  const createMutation = useMutation({
    mutationFn: () => createTask(column.id, { title }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks", column.id] });
      setTitle("");
    },
  });

  function handleCreate(e: FormEvent) {
    e.preventDefault();
    if (!title.trim()) return;
    createMutation.mutate();
  }

  return (
    <div className="bg-slate-100 rounded-lg p-3 w-72 flex-shrink-0">
      <h3 className="font-medium mb-3">{column.name}</h3>

      {isLoading && <p className="text-slate-500 text-sm">Loading...</p>}

      <div className="space-y-2 mb-3">
        {tasks?.map((task) => (
          <div key={task.id} className="bg-white rounded p-3 shadow-sm">
            <p className="text-sm font-medium">{task.title}</p>
            {task.description && <p className="text-xs text-slate-500 mt-1">{task.description}</p>}
          </div>
        ))}
      </div>

      <form onSubmit={handleCreate} className="flex gap-1">
        <input
          type="text"
          placeholder="New task"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="flex-1 border border-slate-300 rounded px-2 py-1 text-sm"
        />
        <button
          type="submit"
          disabled={createMutation.isPending}
          className="bg-slate-900 text-white rounded px-2 py-1 text-sm disabled:opacity-50"
        >
          +
        </button>
      </form>
    </div>
  );
}