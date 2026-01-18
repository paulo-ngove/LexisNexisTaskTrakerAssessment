import Swal from 'sweetalert2';

export const AppSwal = Swal.mixin({
  customClass: {
    confirmButton: 'bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded mx-2 transition-colors',
    cancelButton: 'bg-gray-200 hover:bg-gray-300 text-gray-800 font-bold py-2 px-4 rounded mx-2 transition-colors',
    popup: 'rounded-xl border border-gray-100 shadow-2xl font-sans',
    title: 'text-2xl font-bold text-gray-800',
    htmlContainer: 'text-gray-600'
  },
  buttonsStyling: false 
});