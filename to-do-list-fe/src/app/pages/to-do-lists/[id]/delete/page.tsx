"use client";

import { useParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { API_BASE_URL } from "@/lib/api";
import Link from "next/link";

interface TodoList {
  id: string;
  name: string;
  createdAt: string;
}

export default function DeleteTodoList() {
  const { id } = useParams();
  const router = useRouter();

  const [todoList, setTodoList] = useState<TodoList | null>(null);
  const [loading, setLoading] = useState(true);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  useEffect(() => {
    fetch(`${API_BASE_URL}/api/v1.0/todolists/${id}`)
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch to-do list.");
        return res.json();
      })
      .then((data) => {
        setTodoList(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error(err);
        setError("Could not load to-do list.");
        setLoading(false);
      });
  }, [id]);

  const handleDelete = async () => {
    setDeleting(true);
    setDeleteError(null);

    try {
      const res = await fetch(`${API_BASE_URL}/api/v1.0/todolists/${id}`, {
        method: "DELETE",
      });

      if (!res.ok) {
        const result = await res.json();
        throw new Error(result?.message || "Delete failed");
      }

      router.push("/pages/to-do-lists");
    } catch (err: any) {
      console.error("Delete error:", err);
      setDeleteError("Failed to delete the to-do list.");
    } finally {
      setDeleting(false);
    }
  };

  if (loading)
    return <p className="text-gray-500 text-center">Loading to-do list...</p>;
  if (error)
    return <p className="text-red-500 text-center">{error}</p>;

  return (
    <div className="max-w-md mx-auto mt-10 bg-white p-6 rounded-lg shadow-lg">
      <h1 className="text-2xl font-bold text-gray-900 text-center mb-6">
        Delete To-do List
      </h1>

      {deleteError && (
        <p className="text-red-500 text-center mb-4">{deleteError}</p>
      )}

      <p className="text-center text-gray-900 mb-6">
        Are you sure you want to delete the to-do list{" "}
        <strong>{todoList?.name}</strong> created on{" "}
        <strong>
          {new Date(todoList!.createdAt).toLocaleDateString()}
        </strong>
        ?
      </p>

      <div className="flex justify-center space-x-4">
        <button
          onClick={handleDelete}
          disabled={deleting}
          className="bg-red-500 text-white px-4 py-2 rounded-md hover:bg-red-600 transition disabled:opacity-50"
        >
          {deleting ? "Deleting..." : "Delete"}
        </button>

        <Link
          href="/pages/to-do-lists"
          className="bg-gray-300 px-4 py-2 rounded-md hover:bg-gray-400 transition text-black"
        >
          Cancel
        </Link>
      </div>
    </div>
  );
}
