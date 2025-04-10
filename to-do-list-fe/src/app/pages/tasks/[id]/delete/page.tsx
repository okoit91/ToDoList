"use client";

import { useRouter, useParams } from "next/navigation";
import { useEffect, useState } from "react";
import { API_BASE_URL } from "@/lib/api";
import Link from "next/link";

export default function DeleteTask() {
  const router = useRouter();
  const params = useParams();
  const taskId = params?.id as string;


  const [taskTitle, setTaskTitle] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  // Fetch task title for confirmation UI
  useEffect(() => {
    fetch(`${API_BASE_URL}/api/v1.0/tasks/${taskId}`)
      .then((res) => {
        if (!res.ok) throw new Error("Task not found");
        return res.json();
      })
      .then((data) => setTaskTitle(data.title))
      .catch(() => setError("Failed to load task info."));
  }, [taskId]);

  const handleDelete = async () => {
    try {
      const res = await fetch(`${API_BASE_URL}/api/v1.0/todolists/${taskId}`, {
        method: "DELETE",
      });
  
      if (!res.ok) {
        const text = await res.text(); // fallback instead of .json()
        throw new Error(text);
      }
  
      router.push("/pages/to-do-lists");
    } catch (err: any) {
      console.error("Delete error:", err.message || err);
      alert("Failed to delete the list: " + (err.message || "Unknown error"));
    }
  };

  return (
    <div className="max-w-lg mx-auto p-6 bg-white shadow-md rounded-md mt-10 text-center">
      <h1 className="text-2xl font-bold mb-4 text-red-600">Delete Task</h1>

      {error && <p className="text-red-500 mb-4">{error}</p>}

      {!taskTitle ? (
        <p className="text-gray-500">Loading task details...</p>
      ) : (
        <>
          <p className="mb-6  text-gray-900">
            Are you sure you want to delete the task:
            <br />
            <span className="font-semibold text-gray-900">"{taskTitle}"</span>?
          </p>

          <div className="flex justify-center space-x-4">
            <button
              onClick={handleDelete}
              disabled={deleting}
              className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-md transition disabled:opacity-50"
            >
              {deleting ? "Deleting..." : "Yes, Delete"}
            </button>

            <Link
              href="/pages/tasks"
              className="bg-gray-200 hover:bg-gray-300 text-gray-800 px-4 py-2 rounded-md transition"
            >
              Cancel
            </Link>
          </div>
        </>
      )}
    </div>
  );
}
