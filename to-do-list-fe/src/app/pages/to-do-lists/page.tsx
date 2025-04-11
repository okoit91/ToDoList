"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { API_BASE_URL } from "@/lib/api";
import TodoListCard, { TodoListItem, TaskItem } from "@/components/ToDoListCard";


export default function TodoLists() {
  const [todoLists, setTodoLists] = useState<TodoListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch(`${API_BASE_URL}/api/v1.0/todolists`)
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch todo lists");
        return res.json();
      })
      .then((data) => {
        setTodoLists(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error("Error fetching todo lists:", err);
        setError("Failed to load todo lists.");
        setLoading(false);
      });
  }, []);

  const updateTask = (
    listId: string,
    taskId: string,
    updatedTask: Partial<TaskItem> | null
  ) => {
    const updateTaskInTree = (lists: TodoListItem[]): TodoListItem[] => {
      return lists.map((list) => {
        let updated = { ...list };

        if (list.id === listId) {
          updated.tasks = updatedTask
            ? list.tasks?.map((task) =>
              task.id === taskId ? { ...task, ...updatedTask } : task
            )
            : list.tasks?.filter((task) => task.id !== taskId);
        }

        if (list.subLists && list.subLists.length > 0) {
          updated.subLists = updateTaskInTree(list.subLists);
        }

        return updated;
      });
    };

    setTodoLists((prevLists) => updateTaskInTree(prevLists));
  };

  if (loading) {
    return <p className="text-gray-500 text-center">Loading to-do lists...</p>;
  }

  if (error) {
    return <p className="text-red-500 text-center">{error}</p>;
  }

  const parentLists = todoLists.filter((list) => !list.parentListId);

  return (
    <div className="max-w-8xl mx-auto mt-10 px-4">
      <div
        role="heading"
        aria-level={1}
        className="text-3xl font-bold text-white text-center mb-8"
      >
        To-Do Lists
      </div>

      <div className="text-center mb-6">
        <Link href="/pages/to-do-lists/create" className="text-blue-500 hover:underline">
          Create New To-Do List
        </Link>
      </div>

      {parentLists.length > 0 ? (
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {parentLists.map((list) => (
            <TodoListCard key={list.id} list={list} updateTask={updateTask} />
          ))}
        </div>
      ) : (
        <p className="text-center text-gray-600">No parent to-do lists found.</p>
      )}
    </div>
  );
}
