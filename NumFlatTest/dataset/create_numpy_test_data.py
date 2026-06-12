from pathlib import Path
import numpy as np

out = Path(__file__).resolve().parent
np.save(out / 'numpy_vector_float32.npy', np.array([1.5, -2.25, 3.125, 4.5], dtype=np.float32))
np.save(out / 'numpy_vector_float64.npy', np.array([1.25, -2.5, 3.75, 4.125], dtype=np.float64))
np.save(out / 'numpy_vector_complex128.npy', np.array([1 + 2j, -3.5 + 4.25j, 0 - 1.5j], dtype=np.complex128))
np.save(out / 'numpy_matrix_float64.npy', np.array([[1.0, 2.0, 3.0], [4.5, -5.25, 6.125]], dtype=np.float64))
np.save(out / 'numpy_matrix_complex128.npy', np.array([[1 + 2j, 3 - 4j], [-5.5 + 6.25j, 7.75 - 8.5j]], dtype=np.complex128))
