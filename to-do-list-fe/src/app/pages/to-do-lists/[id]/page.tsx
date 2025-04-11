"use client";

import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import { API_BASE_URL } from "@/lib/api";
import Link from "next/link";
import TodoListCard from "@/components/ToDoListCard";

interface TaskItem {
  id: string;
  title: string;
  description?: string;
  isCompleted: boolean;
  isArchived: boolean;
  dueDate?: string;
  completedAt?: string | null;
}

interface TodoList {
  id: string;
  name: string;
  createdAt: string;
  parentListId?: string | null;
  subLists?: TodoList[];
  tasks?: TaskItem[];
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

  const updateTask = (
    listId: string,
    taskId: string,
    updatedTask: Partial<TaskItem> | null
  ) => {
    if (!data) return;

    const updated = {
      ...data,
      tasks: updatedTask
        ? data.tasks?.map((task) =>
            task.id === taskId ? { ...task, ...updatedTask } : task
          )
        : data.tasks?.filter((task) => task.id !== taskId),
    };

    setData(updated);
  };

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
    <div className="max-w-4xl mx-auto mt-10 px-4">
      <div
        role="heading"
        aria-level={1}
        className="text-3xl font-bold text-white text-center mb-8"
      >
        To-Do List Details
      </div>

      <TodoListCard list={data} updateTask={updateTask} />

      <div className="text-center mt-6">
        <Link href="/pages/to-do-lists" className="text-blue-500 hover:underline">
          Back to To-Do Lists
        </Link>
      </div>
    </div>
  );
}
