import { useParams, Link } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { getBoardById } from "../api/boards";
import { getColumns } from "../api/columns";
import ColumnView from "../components/ColumnView";
import NewColumnForm from "../components/NewColumnForm";

export default function BoardDetailPage() {
  const { projectId, boardId } = useParams<{ projectId: string; boardId: string }>();

  const { data: board } = useQuery({
    queryKey: ["board", projectId, boardId],
    queryFn: () => getBoardById(projectId!, boardId!),
    enabled: !!projectId && !!boardId,
  });

  const { data: columns, isLoading } = useQuery({
    queryKey: ["columns", boardId],
    queryFn: () => getColumns(boardId!),
    enabled: !!boardId,
  });

  return (
    <div className="min-h-screen bg-slate-50 p-8">
      <div className="max-w-6xl mx-auto">
        <Link to={`/projects/${projectId}`} className="text-sm text-slate-600 underline">&larr; Back to project</Link>
        <h1 className="text-2xl font-semibold mt-2 mb-6">{board?.name ?? "Loading..."}</h1>

        {isLoading && <p className="text-slate-600">Loading columns...</p>}

        <div className="flex gap-4 overflow-x-auto pb-4">
          {columns?.map((column) => (
            <ColumnView key={column.id} column={column} />
          ))}
          <NewColumnForm boardId={boardId!} nextOrder={columns?.length ?? 0} />
        </div>
      </div>
    </div>
  );
}