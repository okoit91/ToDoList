"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { API_BASE_URL } from "@/lib/api";

interface ToDoList {
  id: string;
  name: string;
}

interface Task {
  id: string;
  toDoListId: string;
  toDoList: ToDoList;
  title: string;
  description: string;
  dueDate: string;
  isCompleted: boolean;
  completedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

interface TaskHistory {
  id: string;
  taskId: string;
  currentTitle: string;
  completedAt: string;
  revertedAt: string | null;
  task: Task;
}

export default function TaskHistories() {
  const [histories, setHistories] = useState<TaskHistory[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch(`${API_BASE_URL}/api/v1.0/taskHistories`)
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch task histories.");
        return res.json();
      })
      .then((data: TaskHistory[]) => {
        setHistories(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error("Error loading task histories:", err);
        setError("Failed to load task histories.");
        setLoading(false);
      });
  }, []);

  if (loading)
    return <p className="text-gray-500 text-center">Loading task histories...</p>;

  if (error)
    return <p className="text-red-500 text-center">{error}</p>;

  return (
    <div className="max-w-3xl mx-auto mt-10 bg-white p-6 rounded-lg shadow-lg">
      <h1 className="text-2xl font-bold text-gray-900 text-center mb-6">
        Task Histories
      </h1>

      <ul className="space-y-4">
        {histories.length > 0 ? (
          histories.map((history) => (
            <li
              key={history.id}
              className="border-b pb-4 flex justify-between items-start"
            >
              <div>
                <p className="text-lg font-semibold text-gray-800">{history.task.title}</p>
                <p className="text-sm text-gray-600">{history.task.description}</p>
                <p className="text-sm text-gray-500">
                  List: <span className="font-medium">{history.task.toDoList?.name}</span>
                </p>
                <p className="text-sm text-gray-500">
                  Due: {new Date(history.task.dueDate).toLocaleDateString()}
                </p>
                <p className="text-sm font-medium text-green-600">
                  Completed at {new Date(history.completedAt).toLocaleString()}
                </p>
              </div>

              <div className="text-sm text-right space-y-1">
                <Link
                  href={`/pages/task-histories/${history.id}`}
                  className="text-green-500 hover:underline"
                >
                  Details
                </Link>
                {!history.revertedAt && (
                  <button
                    onClick={async () => {
                      const res = await fetch(`${API_BASE_URL}/api/v1.0/taskhistories/revert/${history.id}`, {
                        method: "POST",
                        headers: {
                          "Content-Type": "application/json",
                        },
                      });

                      if (!res.ok) throw new Error("Failed to revert task history.");

                      // Update UI after revert
                      setHistories(prev =>
                        prev.map(h =>
                          h.id === history.id ? { ...h, revertedAt: new Date().toISOString() } : h
                        )
                      );
                    }}
                    className="text-red-500 hover:underline block mt-2"
                  >
                    Revert
                  </button>
                )}
              </div>
            </li>
          ))
        ) : (
          <p className="text-gray-600 text-center">No completed task histories found.</p>
        )}
      </ul>
    </div>
  );
}
