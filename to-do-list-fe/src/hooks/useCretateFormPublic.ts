import { useState } from "react";
import { useRouter } from "next/navigation";

export function useCreateFormPublic<T>(
  initialValues: T,
  apiUrl: string,
  redirectUrl?: string
) {
  const [formValues, setFormValues] = useState<T>(initialValues);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const router = useRouter();

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setFormValues((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const res = await fetch(apiUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formValues),
      });

      if (!res.ok) {
        const errorText = await res.text();
        throw new Error(`Failed to create: ${res.status} ${res.statusText} - ${errorText}`);
      }

      if (redirectUrl) {
        router.push(redirectUrl);
      }
    } catch (err: any) {
      console.error("Error creating:", err);
      setError("Something went wrong.");
    } finally {
      setLoading(false);
    }
  };

  return { formValues, setFormValues, handleChange, handleSubmit, error, loading };
}