import { createClient } from "https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2/+esm";

const SUPABASE_URL = "https://ekxbhwxeavvovuywypha.supabase.co";
const SUPABASE_ANON_KEY =
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImVreGJod3hlYXZ2b3Z1eXd5cGhhIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njc5NzA3MTEsImV4cCI6MjA4MzU0NjcxMX0.TnaOGtovhmYqWZy-n3lY96dDXR_RgnK4q6Nfs6bFN8s";

export const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY);
