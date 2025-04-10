"use client";

import { useState } from "react";

export default function TodoList() {
  const [todos, setTodos] = useState<string[]>([]);
  const [newTodo, setNewTodo] = useState("");

  const addTodo = () => {
    if (newTodo.trim() !== "") {
      setTodos([...todos, newTodo.trim()]);
      setNewTodo("");
    }
  };

  const removeTodo = (index: number) => {
    const updated = [...todos];
    updated.splice(index, 1);
    setTodos(updated);
  };

  return (
    <div className="max-w-md mx-auto mt-10 bg-white p-6 rounded-lg shadow-lg">
      <h2 className="text-2xl font-bold text-gray-900 text-center mb-6">
        Todo List
      </h2>

      {/* Input + Add Button */}
      <div className="flex mb-4">
        <input
          type="text"
          value={newTodo}
          onChange={(e) => setNewTodo(e.target.value)}
          placeholder="Enter a new task"
          className="flex-grow p-2 border border-gray-300 rounded-l-md focus:ring focus:ring-blue-300"
        />
        <button
          onClick={addTodo}
          className="px-4 bg-blue-600 text-white rounded-r-md hover:bg-blue-700 transition"
        >
          Add
        </button>
      </div>

      {/* Todo List */}
      <ul className="space-y-2">
        {todos.map((todo, index) => (
          <li
            key={index}
            className="flex justify-between items-center p-2 bg-gray-50 border border-gray-200 rounded"
          >
            <span className="text-gray-800">{todo}</span>
            <button
              onClick={() => removeTodo(index)}
              className="text-red-500 hover:text-red-700 text-sm"
            >
              Remove
            </button>
          </li>
        ))}
        {todos.length === 0 && (
          <li className="text-center text-gray-400 text-sm">No tasks yet. Add something!</li>
        )}
      </ul>
    </div>
  );
}
