"use client";

import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import { API_BASE_URL } from "@/lib/api";
import Link from "next/link";

interface TodoList {
  id: string;
  name: string;
  createdAt: string;
  parentListId?: string | null;
  subLists?: string[];
}

export default function TodoListDetails() {
  const { id } = useParams();
  const [data, setData] = useState<TodoList | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch(`${API_BASE_URL}/api/v1.0/todolists/${id}`)
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch to-do list.");
        return res.json();
      })
      .then((data) => {
        setData(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error("Error loading to-do list:", err);
        setError("Failed to load to-do list.");
        setLoading(false);
      });
  }, [id]);

  if (loading) {
    return <p className="text-gray-500 text-center">Loading to-do list...</p>;
  }

  if (error) {
    return <p className="text-red-500 text-center">{error}</p>;
  }

  if (!data) {
    return <p className="text-gray-500 text-center">To-do list not found.</p>;
  }

  return (
    <div className="max-w-lg mx-auto p-6 bg-white shadow-md rounded-md mt-10">
      <h1 className="text-2xl font-bold mb-6 text-center">To-do List Details</h1>

      <div className="mb-4 space-y-2 text-gray-900">
        <p>
          <strong>Name:</strong> {data.name}
        </p>
        <p>
          <strong>Created At:</strong>{" "}
          {new Date(data.createdAt).toLocaleString()}
        </p>
        {data.parentListId && (
          <p>
            <strong>Parent List ID:</strong> {data.parentListId}
          </p>
        )}
        {data.subLists && data.subLists.length > 0 && (
          <p>
            <strong>SubLists:</strong> {data.subLists.join(", ")}
          </p>
        )}
      </div>

      <div className="text-center mt-4">
        <Link href="/pages/to-do-lists" className="text-blue-500 hover:underline">
          Back to To-do Lists
        </Link>
      </div>
    </div>
  );
}
