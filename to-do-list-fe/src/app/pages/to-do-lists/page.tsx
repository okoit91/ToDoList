"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { API_BASE_URL } from "@/lib/api";

interface TaskItem {
  id: string;
  title: string;
  description?: string;
  isCompleted: boolean;
  isArchived: boolean;
  dueDate?: string;
  completedAt?: string | null;

}

export interface TodoListItem {
  id: string;
  name: string;
  createdAt: string;
  parentListId?: string | null;
  subLists?: TodoListItem[];
  tasks?: TaskItem[];
}

function TodoListCard({
  list,
  updateTask,
}: {
  list: TodoListItem;
  updateTask: (listId: string, taskId: string, updatedTask: Partial<TaskItem> | null) => void;
}): React.ReactElement {
  return (
    <div className="w-full bg-white p-5 rounded-lg shadow-md border mb-4">
      <div className="flex items-center justify-between mb-2">
        <h2 className="text-xl font-semibold text-gray-800 truncate max-w-[60%]">{list.name}</h2>
        <div className="flex flex-nowrap space-x-2 shrink-0">
          <Link
            href={`/pages/tasks/create?listId=${list.id}`}
            className="text-sm bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600 transition"
          >
            Add Task
          </Link>
          <Link
            href={`/pages/to-do-lists/create?parentId=${list.id}`}
            className="text-sm bg-purple-500 text-white px-3 py-1 rounded hover:bg-purple-600 transition"
          >
            Add Sub-list
          </Link>
        </div>
      </div>
      <p className="text-sm text-gray-500 mb-1">
        Created: {new Date(list.createdAt).toLocaleDateString()}
      </p>

      {list.tasks && list.tasks.filter(task => !task.isArchived).length > 0 && (
        <div className="mt-3">
          <p className="text-sm font-semibold text-gray-700 mb-2">Tasks:</p>
          <div className="grid gap-3">
            {list.tasks
              .filter(task => !task.isArchived)
              .map((task) => (
                <div
                  key={task.id}
                  className="border rounded-lg p-3 bg-gray-50 shadow-sm flex justify-between items-start"
                >
                  <div>
                    <h3
                      className={`text-md font-semibold ${task.isCompleted ? "line-through text-gray-400" : "text-gray-800"
                        }`}
                    >
                      {task.title}
                    </h3>
                    {task.description && (
                      <p className="text-sm text-gray-600 mt-1">{task.description}</p>
                    )}
                  </div>

                  <div className="flex flex-nowrap items-center space-x-2 ml-4">
                    {!task.isCompleted ? (
                      <button
                        onClick={async () => {
                          try {
                            const res = await fetch(`${API_BASE_URL}/api/v1.0/tasks/${task.id}`, {
                              method: "PUT",
                              headers: { "Content-Type": "application/json" },
                              body: JSON.stringify({ ...task, isCompleted: true }),
                            });
                            if (!res.ok) throw new Error("Failed to mark task as complete");
                            updateTask(list.id, task.id, {
                              isCompleted: true,
                              completedAt: new Date().toISOString(),
                            });
                          } catch (err) {
                            console.error("Error completing task:", err);
                            alert("Could not complete the task.");
                          }
                        }}
                        className="text-green-600 hover:text-green-800"
                        title="Mark as completed"
                      >
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          className="w-7 h-7"
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke="currentColor"
                          strokeWidth={2}
                        >
                          <path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" />
                        </svg>
                      </button>
                    ) : (
                      <>
                        {/* Undo Button */}
                        <button
                          onClick={async () => {
                            try {
                              const res = await fetch(`${API_BASE_URL}/api/v1.0/tasks/${task.id}`, {
                                method: "PUT",
                                headers: { "Content-Type": "application/json" },
                                body: JSON.stringify({
                                  ...task,
                                  isCompleted: false,
                                  completedAt: null,
                                }),
                              });
                              if (!res.ok) throw new Error("Failed to undo task");
                              updateTask(list.id, task.id, {
                                isCompleted: false,
                                completedAt: null,
                              });
                            } catch (err) {
                              console.error("Error undoing task:", err);
                              alert("Could not undo the task.");
                            }
                          }}
                          className="text-yellow-600 hover:text-yellow-800"
                          title="Undo"
                        >
                          <svg xmlns="http://www.w3.org/2000/svg" className="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="M9 19c-4-2.5-4-10 4-10h6m0 0l-3-3m3 3l-3 3" />
                          </svg>
                        </button>
                        {/* Archive Button */}
                        <button
                          onClick={async () => {
                            try {
                              // 1. Mark task as archived
                              const updatedTask = { ...task, isArchived: true };

                              const updateRes = await fetch(`${API_BASE_URL}/api/v1.0/tasks/${task.id}`, {
                                method: "PUT",
                                headers: { "Content-Type": "application/json" },
                                body: JSON.stringify(updatedTask),
                              });

                              if (!updateRes.ok) throw new Error("Failed to archive task");

                              // 2. Create TaskHistory entry
                              const historyRes = await fetch(`${API_BASE_URL}/api/v1.0/taskhistories`, {
                                method: "POST",
                                headers: { "Content-Type": "application/json" },
                                body: JSON.stringify({
                                  taskId: task.id,
                                  currentTitle: task.title,
                                  completedAt: new Date().toISOString(),
                                }),
                              });

                              if (!historyRes.ok) throw new Error("Failed to create task history");

                              updateTask(list.id, task.id, null); // remove from UI
                            } catch (err) {
                              console.error("Error archiving task:", err);
                              alert("Could not archive the task.");
                            }
                          }}
                          className="text-gray-500 hover:text-gray-700"
                          title="Archive"
                        >
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            className="w-6 h-6"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke="currentColor"
                            strokeWidth={2}
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              d="M4 4h16v4H4V4zm1 5h14v11H5V9z"
                            />
                          </svg>
                        </button>
                      </>
                    )}
                  </div>
                </div>
              ))}
          </div>
        </div>
      )}

      {list.subLists && list.subLists.length > 0 && (
        <div className="mt-4 ml-4 pl-4">
          {list.subLists.map((subList) => (
            <TodoListCard key={subList.id} list={subList} updateTask={updateTask} />
          ))}
        </div>
      )}

      <div className="mt-4 space-x-3">
        <Link href={`/pages/to-do-lists/${list.id}`} className="text-green-500 hover:underline text-sm">
          Details
        </Link>
        <Link href={`/pages/to-do-lists/${list.id}/edit`} className="text-blue-500 hover:underline text-sm">
          Edit
        </Link>
        <Link href={`/pages/to-do-lists/${list.id}/delete`} className="text-red-500 hover:underline text-sm">
          Delete
        </Link>
      </div>
    </div>
  );
}

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
          Create New To-do List
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
