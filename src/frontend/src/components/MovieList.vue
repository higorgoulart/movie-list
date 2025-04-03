<template>
  <div class="min-h-screen">
    <header>
      <nav class="container mx-auto px-4 py-6">
        <h1 class="text-3xl font-bold text-transparent bg-clip-text bg-gradient-to-r from-purple-400 to-pink-600">
          Movie Explorer
        </h1>
      </nav>
    </header>

    <main class="container mx-auto px-4 py-8">
      <div class="mb-8 bg-gray-800 rounded-xl p-6 shadow-lg">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <input
            v-model="filters.title"
            placeholder="Search movies..."
            class="bg-gray-700 rounded-lg p-3 text-white focus:outline-none focus:ring-2 focus:ring-purple-500"
            @input="debouncedLoadMovies"
          >
          <input
            v-model.number="filters.year"
            type="number"
            placeholder="Release year"
            min="1900"
            :max="new Date().getFullYear()"
            class="bg-gray-700 rounded-lg p-3 text-white focus:outline-none focus:ring-2 focus:ring-purple-500"
            @input="loadMovies"
          >
          <input
            v-model.number="filters.minVote"
            type="number"
            step="0.1"
            placeholder="Min rating (0-10)"
            min="0"
            max="10"
            class="bg-gray-700 rounded-lg p-3 text-white focus:outline-none focus:ring-2 focus:ring-purple-500"
            @input="loadMovies"
          >
        </div>
      </div>

      <div v-if="loading" class="text-center py-12">
        <div class="animate-pulse flex flex-col items-center">
          <div class="h-12 w-12 bg-purple-500 rounded-full mb-4"></div>
          <div class="h-4 bg-gray-700 rounded w-1/4 mb-2"></div>
        </div>
      </div>

      <div v-else-if="error" class="text-red-400 text-center p-6 bg-red-900/20 rounded-lg">
        ⚠️ Error loading movies: {{ error }}
      </div>

      <div v-else>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          <article
            v-for="movie in movies.results"
            :key="movie.id"
            class="group relative bg-gray-800 rounded-xl overflow-hidden shadow-lg hover:shadow-2xl transition-all duration-300"
          >
            <img
              :src="posterUrl(movie.posterPath)"
              :alt="movie.title"
              class="w-full h-80 object-cover transition-transform duration-300 group-hover:scale-105"
              @error="handleImageError"
            >
            <div class="p-4">
              <h2 class="text-xl font-bold mb-2 truncate">{{ movie.title }}</h2>
              
              <div class="flex items-center justify-between mb-3">
                <span class="text-sm text-gray-400">
                  {{ formatDate(movie.releaseDate) }}
                </span>
                <div class="flex items-center space-x-1">
                  <span class="text-yellow-400">★</span>
                  <span>{{ movie.voteAverage.toFixed(1) }}</span>
                  <span class="text-gray-400 text-sm">({{ movie.voteCount }})</span>
                </div>
              </div>

              <p class="text-gray-300 text-sm line-clamp-3 mb-4">{{ movie.overview }}</p>
              
              <div class="flex items-center justify-between text-sm text-gray-400">
                <span>Popularity: {{ Math.round(movie.popularity) }}</span>
                <span>ID: {{ movie.id }}</span>
              </div>
            </div>
          </article>
        </div>

        <div class="mt-8 flex flex-col sm:flex-row items-center justify-between gap-4">
          <div class="text-gray-400">
            Showing page {{ movies.page }} of {{ movies.totalPages }}
          </div>
          <div class="flex gap-2 mt-2">
            <button
              @click="previousPage"
              :disabled="movies.page === 1"
              class="px-6 py-2 rounded-lg bg-gray-700 hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              ← Previous
            </button>
            <button
              @click="nextPage"
              :disabled="movies.page === movies.totalPages"
              class="px-6 py-2 rounded-lg bg-gray-700 hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              Next →
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import axios from 'axios';

interface Movie {
  id: number;
  title: string;
  overview: string;
  releaseDate: string;
  popularity: number;
  voteAverage: number;
  voteCount: number;
  posterPath: string;
}

interface MovieResponse {
  page: number;
  totalPages: number;
  results: Movie[];
}

const movies = reactive<MovieResponse>({
  page: 1,
  totalPages: 1,
  results: []
});

const filters = reactive({
  title: '',
  year: null as number | null,
  minVote: null as number | null
});

const loading = ref(true);
const error = ref<string | null>(null);
let debounceTimeout: number;

const API_BASE = 'http://localhost:5001/api';
const IMAGE_BASE = 'https://image.tmdb.org/t/p/w500';

const posterUrl = (path: string) => 
  path ? `${IMAGE_BASE}${path}` : '/placeholder-movie.jpg';

const formatDate = (dateString: string) => 
  new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  });

const handleImageError = (event: Event) => {
  const img = event.target as HTMLImageElement;
  img.src = '/placeholder-movie.jpg';
};

const debouncedLoadMovies = () => {
  clearTimeout(debounceTimeout);
  debounceTimeout = setTimeout(loadMovies, 500);
};

const loadMovies = async () => {
  try {
    loading.value = true;
    error.value = null;
    
    const response = await axios.get(`${API_BASE}/movies`, {
      params: {
        page: movies.page,
        title: filters.title || undefined,
        year: filters.year || undefined,
        minVote: filters.minVote || undefined
      }
    });

    console.log(response)

    Object.assign(movies, response.data);
  } catch (err) {
    error.value = axios.isAxiosError(err) 
      ? err.response?.data?.message || err.message
      : 'Unknown error occurred';
  } finally {
    loading.value = false;
  }
};

// Pagination controls
const nextPage = () => {
  if (movies.page < movies.totalPages) {
    movies.page++;
    loadMovies();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
};

const previousPage = () => {
  if (movies.page > 1) {
    movies.page--;
    loadMovies();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
};

loadMovies();
</script>

<style>

</style>