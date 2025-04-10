"use client";

import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import { API_BASE_URL } from "@/lib/api";
import Link from "next/link";

interface ToDoList {
  id: string;
  name: string;
}

interface Task {
  id: string;
  title: string;
  description: string;
  dueDate: string;
  isCompleted: boolean;
  isArchived: boolean;
  completedAt: string | null;
  createdAt: string;
  updatedAt: string;
  toDoList: ToDoList;
}

interface TaskHistory {
  id: string;
  taskId: string;
  currentTitle: string;
  completedAt: string;
  revertedAt: string | null;
  task: Task;
}

export default function TaskHistoryDetails() {
  const { id } = useParams();
  const [history, setHistory] = useState<TaskHistory | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch(`${API_BASE_URL}/api/v1.0/taskhistories/${id}`)
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch task history.");
        return res.json();
      })
      .then((data) => {
        setHistory(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error("Error loading task history:", err);
        setError("Failed to load task history.");
        setLoading(false);
      });
  }, [id]);

  if (loading)
    return <p className="text-gray-500 text-center">Loading task history...</p>;

  if (error)
    return <p className="text-red-500 text-center">{error}</p>;

  if (!history)
    return <p className="text-gray-500 text-center">Task history not found.</p>;

  return (
    <div className="max-w-lg mx-auto p-6 bg-white shadow-md rounded-md mt-10">
      <h1 className="text-2xl font-bold mb-6 text-center">Task History Details</h1>

      <div className="mb-4 space-y-2 text-gray-900">
        <p><strong>Task Title:</strong> {history.currentTitle}</p>
        <p><strong>Completed At:</strong> {new Date(history.completedAt).toLocaleString()}</p>
        {history.revertedAt && (
          <p><strong>Reverted At:</strong> {new Date(history.revertedAt).toLocaleString()}</p>
        )}
      </div>

      <div className="text-center mt-4 space-x-4">
        <Link href="/pages/task-histories" className="text-gray-600 hover:underline">
          Back to Task Histories
        </Link>
      </div>
    </div>
  );
}
