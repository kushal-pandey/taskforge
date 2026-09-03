import { useState, type FormEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createColumn } from "../api/columns";

interface NewColumnFormProps {
  boardId: string;
  nextOrder: number;
}

export default function NewColumnForm({ boardId, nextOrder }: NewColumnFormProps) {
  const [name, setName] = useState("");
  const queryClient = useQueryClient();

  const createMutation = useMutation({
    mutationFn: () => createColumn(boardId, { name, order: nextOrder }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["columns", boardId] });
      setName("");
    },
  });

  function handleCreate(e: FormEvent) {
    e.preventDefault();
    if (!name.trim()) return;
    createMutation.mutate();
  }

  return (
    <form onSubmit={handleCreate} className="bg-white rounded-lg p-3 w-72 flex-shrink-0 h-fit">
      <input
        type="text"
        placeholder="New column name"
        value={name}
        onChange={(e) => setName(e.target.value)}
        className="w-full border border-slate-300 rounded px-2 py-1 text-sm mb-2"
      />
      <button
        type="submit"
        disabled={createMutation.isPending}
        className="w-full bg-slate-900 text-white rounded px-2 py-1 text-sm disabled:opacity-50"
      >
        {createMutation.isPending ? "Adding..." : "+ Add Column"}
      </button>
    </form>
  );
}