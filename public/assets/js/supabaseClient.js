import { createClient } from "https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2/+esm";

const SUPABASE_URL = "https://ekxbhwxeavvovuywypha.supabase.co";
const SUPABASE_ANON_KEY = "sb_publishable_uL2VNZySwJH4qGCM3TASiA_hQI2nEFE";

export const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY);
